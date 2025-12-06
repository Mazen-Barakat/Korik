using Korik.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Korik.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Get all notifications for the current user
        /// </summary>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get all notifications",
            Description = "Retrieves all notifications for the authenticated user, ordered by creation date (newest first)."
        )]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _notificationService.GetUserNotificationsAsync(userId);
            return ApiResponse.FromResult(this, result);
        }

        /// <summary>
        /// Get unread notification count
        /// </summary>
        [HttpGet("unread-count")]
        [SwaggerOperation(
            Summary = "Get unread notification count",
            Description = "Returns the count of unread notifications for the authenticated user."
        )]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _notificationService.GetUnreadCountAsync(userId);
            return ApiResponse.FromResult(this, result);
        }

        /// <summary>
        /// Mark a notification as read
        /// </summary>
        [HttpPut("{notificationId}/read")]
        [SwaggerOperation(
            Summary = "Mark notification as read",
            Description = "Marks a specific notification as read."
        )]
        public async Task<IActionResult> MarkAsRead([FromRoute] int notificationId)
        {
            var result = await _notificationService.MarkAsReadAsync(notificationId);
            return ApiResponse.FromResult(this, result);
        }

        /// <summary>
        /// Get pending appointment confirmation dialogs for the current user
        /// </summary>
        /// <remarks>
        /// Returns all pending confirmation notifications where:
        /// - The booking is still in "Confirmed" status
        /// - The confirmation window hasn't expired
        /// - Used to show confirmation dialogs when page loads or when user clicks on notification from panel
        /// </remarks>
        [HttpGet("pending-confirmations")]
        [SwaggerOperation(
            Summary = "Get pending appointment confirmations",
            Description = "Retrieves all pending appointment confirmation requests for the authenticated user. " +
                          "These are bookings that require confirmation and the deadline hasn't passed. " +
                          "Use this to restore confirmation dialogs on page reload or when clicking notifications from the panel."
        )]
        public async Task<IActionResult> GetPendingConfirmations()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _notificationService.GetPendingConfirmationsAsync(userId);
            return ApiResponse.FromResult(this, result);
        }

        /// <summary>
        /// Get notification with full booking details for restoring confirmation dialog
        /// </summary>
        /// <remarks>
        /// Used when user clicks on a notification from the notification panel to restore the confirmation dialog.
        /// Returns all information needed to display the dialog with preserved time and confirmation status.
        /// </remarks>
        [HttpGet("{notificationId}/details")]
        [SwaggerOperation(
            Summary = "Get notification with booking details",
            Description = "Retrieves a specific notification with full booking details. " +
                          "Used to restore confirmation dialog when user clicks on a notification from the panel. " +
                          "Includes remaining time, confirmation status, and whether the dialog can still be shown."
        )]
        public async Task<IActionResult> GetNotificationWithDetails([FromRoute] int notificationId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _notificationService.GetNotificationWithBookingDetailsAsync(notificationId, userId);
            return ApiResponse.FromResult(this, result);
        }
    }
}
