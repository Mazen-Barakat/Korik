using Korik.Application;
using Korik.Domain;
using Microsoft.EntityFrameworkCore;

namespace Korik.Infrastructure
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        private readonly Korik _context;

        public PaymentRepository(Korik context) : base(context)
        {
            _context = context;
        }

        public async Task<Payment?> GetByStripePaymentIntentIdAsync(string paymentIntentId)
        {
            return await _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.WorkShopProfile)
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Car)
                        .ThenInclude(c => c.CarOwnerProfile)
                .FirstOrDefaultAsync(p => p.StripePaymentIntentId == paymentIntentId);
        }

        public async Task<Payment?> GetByBookingIdAsync(int bookingId)
        {
            return await _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.WorkShopProfile)
                .Include(p => p.Booking)
                    .ThenInclude(b => b.WorkshopService)
                .FirstOrDefaultAsync(p => p.BookingId == bookingId);
        }

        public async Task<List<Payment>> GetPendingPayoutsAsync()
        {
            return await _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.WorkShopProfile)
                .Include(p => p.Booking)
                    .ThenInclude(b => b.WorkshopService)
                        .ThenInclude(ws => ws.Service)
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Car)
                        .ThenInclude(c => c.CarOwnerProfile)
                            .ThenInclude(cop => cop.ApplicationUser)
                .Where(p => p.StripePaymentStatus == StripePaymentStatus.Succeeded
                         && !p.IsPaidOut
                         && p.Booking.Status == BookingStatus.Completed)
                .OrderBy(p => p.PaidAt)
                .ToListAsync();
        }
    }
}
