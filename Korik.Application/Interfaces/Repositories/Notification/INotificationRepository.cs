using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(string userId);
        Task<int> GetUnreadCountByUserIdAsync(string userId);
        Task<bool> MarkAsReadAsync(int notificationId);
        
        /// <summary>
        /// Gets pending confirmation notifications with booking details
        /// </summary>
        Task<IEnumerable<Notification>> GetPendingConfirmationNotificationsAsync(string userId);
        
        /// <summary>
        /// Gets a specific notification with booking and related entity details
        /// </summary>
        Task<Notification?> GetNotificationWithBookingDetailsAsync(int notificationId, string userId);
    }
}
