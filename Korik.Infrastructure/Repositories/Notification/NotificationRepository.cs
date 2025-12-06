using Korik.Application;
using Korik.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Infrastructure
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        private readonly Korik _context;

        public NotificationRepository(Korik context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.ReceiverId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountByUserIdAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.ReceiverId == userId && !n.IsRead)
                .CountAsync();
        }

        public async Task<bool> MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null)
                return false;

            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Notification>> GetPendingConfirmationNotificationsAsync(string userId)
        {
            return await _context.Notifications
                .Include(n => n.Booking)
                    .ThenInclude(b => b!.Car)
                        .ThenInclude(c => c.CarOwnerProfile)
                .Include(n => n.Booking)
                    .ThenInclude(b => b!.WorkShopProfile)
                .Include(n => n.Booking)
                    .ThenInclude(b => b!.WorkshopService)
                        .ThenInclude(ws => ws.Service)
                .Where(n => n.ReceiverId == userId 
                            && n.Type == NotificationType.AppointmentConfirmationRequest
                            && n.BookingId.HasValue
                            && n.Booking != null
                            && n.Booking.Status == BookingStatus.Confirmed) // Only if booking still needs confirmation
                .OrderByDescending(n => n.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Notification?> GetNotificationWithBookingDetailsAsync(int notificationId, string userId)
        {
            return await _context.Notifications
                .Include(n => n.Booking)
                    .ThenInclude(b => b!.Car)
                        .ThenInclude(c => c.CarOwnerProfile)
                .Include(n => n.Booking)
                    .ThenInclude(b => b!.WorkShopProfile)
                .Include(n => n.Booking)
                    .ThenInclude(b => b!.WorkshopService)
                        .ThenInclude(ws => ws.Service)
                .Where(n => n.Id == notificationId && n.ReceiverId == userId)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}
