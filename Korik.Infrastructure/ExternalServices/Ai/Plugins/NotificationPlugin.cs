using Korik.Application;
using Korik.Domain;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text;

namespace Korik.Infrastructure
{
    /// <summary>
    /// Plugin for handling user notification queries via AI assistant.
    /// </summary>
    public class NotificationPlugin
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationPlugin(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        [KernelFunction("GetMyNotifications")]
        [Description("Gets all notifications for the current user. Use this when user asks about their notifications, alerts, or messages.")]
        public async Task<string> GetMyNotificationsAsync(
            [Description("The authenticated user's ID")] string userId)
        {
            var notifications = await _notificationRepository.GetNotificationsByUserIdAsync(userId);
            var notificationList = notifications.ToList();

            if (!notificationList.Any())
            {
                return "You don't have any notifications yet.";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"?? You have {notificationList.Count} notification(s):");
            sb.AppendLine();

            foreach (var notification in notificationList.Take(15))
            {
                var readStatus = notification.IsRead ? "?" : "?? NEW";
                var typeIcon = GetNotificationIcon(notification.Type);

                sb.AppendLine($"{typeIcon} {readStatus} - {notification.Message}");
                sb.AppendLine($"   Date: {notification.CreatedAt:yyyy-MM-dd HH:mm}");

                if (notification.BookingId.HasValue)
                {
                    sb.AppendLine($"   Related to Booking #{notification.BookingId}");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        [KernelFunction("GetUnreadNotificationCount")]
        [Description("Gets the count of unread notifications. Use this when user asks how many new notifications they have.")]
        public async Task<string> GetUnreadNotificationCountAsync(
            [Description("The authenticated user's ID")] string userId)
        {
            var unreadCount = await _notificationRepository.GetUnreadCountByUserIdAsync(userId);

            if (unreadCount == 0)
            {
                return "?? You have no unread notifications. You're all caught up!";
            }

            return $"?? You have {unreadCount} unread notification(s).";
        }

        [KernelFunction("GetPendingConfirmations")]
        [Description("Gets pending appointment confirmations that require user action.")]
        public async Task<string> GetPendingConfirmationsAsync(
            [Description("The authenticated user's ID")] string userId)
        {
            var pendingNotifications = await _notificationRepository.GetPendingConfirmationNotificationsAsync(userId);
            var pendingList = pendingNotifications.ToList();

            if (!pendingList.Any())
            {
                return "? You have no pending appointment confirmations.";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"? You have {pendingList.Count} pending confirmation(s):");
            sb.AppendLine();

            foreach (var notification in pendingList)
            {
                sb.AppendLine($"?? {notification.Message}");
                sb.AppendLine($"   Received: {notification.CreatedAt:yyyy-MM-dd HH:mm}");

                if (notification.BookingId.HasValue)
                {
                    sb.AppendLine($"   Booking ID: #{notification.BookingId}");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        [KernelFunction("GetRecentNotifications")]
        [Description("Gets the most recent notifications.")]
        public async Task<string> GetRecentNotificationsAsync(
            [Description("The authenticated user's ID")] string userId,
            [Description("Number of recent notifications (default 5)")] int count = 5)
        {
            var allNotifications = await _notificationRepository.GetNotificationsByUserIdAsync(userId);
            var recentNotifications = allNotifications
                .OrderByDescending(n => n.CreatedAt)
                .Take(count)
                .ToList();

            if (!recentNotifications.Any())
            {
                return "You don't have any notifications yet.";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"?? Your {recentNotifications.Count} most recent notification(s):");
            sb.AppendLine();

            foreach (var notification in recentNotifications)
            {
                var readStatus = notification.IsRead ? "?" : "?? NEW";
                var typeIcon = GetNotificationIcon(notification.Type);
                var timeAgo = GetTimeAgo(notification.CreatedAt);

                sb.AppendLine($"{typeIcon} {readStatus} {notification.Message}");
                sb.AppendLine($"   {timeAgo}");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static string GetNotificationIcon(NotificationType type)
        {
            return type switch
            {
                NotificationType.BookingCreated => "??",
                NotificationType.BookingAccepted => "?",
                NotificationType.BookingRejected => "?",
                NotificationType.BookingCancelled => "??",
                NotificationType.CarReadyForPickup => "??",
                NotificationType.BookingCompleted => "??",
                NotificationType.AppointmentReminder => "?",
                NotificationType.AppointmentConfirmationRequest => "?",
                NotificationType.ResponseStatusChanged => "??",
                _ => "??"
            };
        }

        private static string GetTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime;

            if (timeSpan.TotalMinutes < 1) return "Just now";
            if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes} minute(s) ago";
            if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours} hour(s) ago";
            if (timeSpan.TotalDays < 7) return $"{(int)timeSpan.TotalDays} day(s) ago";

            return dateTime.ToString("yyyy-MM-dd");
        }
    }
}
