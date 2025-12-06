using Korik.Application;
using Korik.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Korik.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestNotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IUserConnectionManager _connectionManager;

        public TestNotificationController(
            INotificationService notificationService,
            IUserConnectionManager connectionManager)
        {
            _notificationService = notificationService;
            _connectionManager = connectionManager;
        }

        /// <summary>
        /// Test endpoint to manually send a notification via SignalR
        /// Use this to verify real-time notification delivery is working
        /// </summary>
        [HttpPost("test-push")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Test SignalR notification delivery",
            Description = "Manually trigger a notification to test SignalR real-time delivery. " +
                          "The notification will be sent to the specified user ID. " +
                          "Check backend logs and frontend console to verify delivery."
        )]
        public async Task<IActionResult> TestPush([FromBody] TestPushNotificationDto dto)
        {
            Console.WriteLine("========================================");
            Console.WriteLine("?? TEST NOTIFICATION TRIGGERED");
            Console.WriteLine($"  Target User: {dto.UserId}");
            Console.WriteLine($"  Message: {dto.Message}");
            Console.WriteLine($"  BookingId: {dto.BookingId}");
            Console.WriteLine("========================================");

            var result = await _notificationService.SendNotificationAsync(
                senderId: "system-test",
                receiverId: dto.UserId,
                message: dto.Message,
                type: NotificationType.AppointmentConfirmationRequest,
                bookingId: dto.BookingId,
                title: "Test Notification",
                priority: "high"
            );

            return ApiResponse.FromResult(this, result);
        }

        /// <summary>
        /// Check if a user is currently connected to SignalR
        /// </summary>
        [HttpGet("check-connection/{userId}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Check if user is connected to SignalR",
            Description = "Verify if a specific user has an active SignalR connection."
        )]
        public IActionResult CheckConnection([FromRoute] string userId)
        {
            var isConnected = _connectionManager.IsUserConnected(userId);
            var connections = _connectionManager.GetConnections(userId);

            return Ok(new
            {
                userId = userId,
                isConnected = isConnected,
                connectionCount = connections.Count,
                connectionIds = connections
            });
        }
    }

    public class TestPushNotificationDto
    {
        /// <summary>
        /// The user ID to send the notification to (must match the JWT NameIdentifier claim)
        /// </summary>
        public string UserId { get; set; } = string.Empty;
        
        /// <summary>
        /// The message to display in the notification
        /// </summary>
        public string Message { get; set; } = "Test notification from backend - if you see this, SignalR is working!";
        
        /// <summary>
        /// Optional booking ID to associate with the notification
        /// </summary>
        public int? BookingId { get; set; } = 999;
    }
}
