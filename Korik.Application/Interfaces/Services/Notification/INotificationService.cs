using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public interface INotificationService
    {
        Task<ServiceResult<NotificationDto>> SendNotificationAsync(string senderId, string receiverId, string message, NotificationType type, int? bookingId = null);
        Task<ServiceResult<IEnumerable<NotificationDto>>> GetUserNotificationsAsync(string userId);
        Task<ServiceResult<int>> GetUnreadCountAsync(string userId);
        Task<ServiceResult<bool>> MarkAsReadAsync(int notificationId);
    }
}
