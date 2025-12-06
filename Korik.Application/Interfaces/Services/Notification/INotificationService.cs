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
        Task<ServiceResult<NotificationDto>> SendNotificationAsync(
            string senderId, 
            string receiverId, 
            string message, 
            NotificationType type, 
            int? bookingId = null,
            string? title = null,
            string? priority = null,
            DateTime? confirmationDeadline = null);

        /// <summary>
        /// Sends a booking response changed event via SignalR
        /// </summary>
        Task<ServiceResult<bool>> SendBookingResponseChangedAsync(
            string receiverId,
            int bookingId,
            string bookingReference,
            int previousStatus,
            int newStatus,
            string changedBy);

        /// <summary>
        /// Sends a confirmation status update event via SignalR to notify parties about confirmation progress
        /// Used when one party confirms and we need to update the other party's dialog
        /// </summary>
        Task<ServiceResult<bool>> SendConfirmationStatusUpdateAsync(
            string receiverId,
            int bookingId,
            bool? carOwnerConfirmed,
            bool? workshopOwnerConfirmed,
            string newBookingStatus,
            bool shouldDismissDialog);

        /// <summary>
        /// Gets pending confirmation notifications for a user (not yet confirmed bookings)
        /// Used to restore confirmation dialogs when user clicks on notification from panel
        /// </summary>
        Task<ServiceResult<IEnumerable<PendingConfirmationDto>>> GetPendingConfirmationsAsync(string userId);

        /// <summary>
        /// Gets a specific notification with full booking details for restoring confirmation dialog
        /// </summary>
        Task<ServiceResult<PendingConfirmationDto>> GetNotificationWithBookingDetailsAsync(int notificationId, string userId);
            
        Task<ServiceResult<IEnumerable<NotificationDto>>> GetUserNotificationsAsync(string userId);
        Task<ServiceResult<int>> GetUnreadCountAsync(string userId);
        Task<ServiceResult<bool>> MarkAsReadAsync(int notificationId);
    }
}
