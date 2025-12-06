using Korik.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Infrastructure
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly IUserConnectionManager _connectionManager;
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(IUserConnectionManager connectionManager, ILogger<NotificationHub> logger)
        {
            _connectionManager = connectionManager;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userIdentifier = Context.UserIdentifier; // Should match userId if IUserIdProvider is configured

            _logger.LogInformation("========================================");
            _logger.LogInformation("?? SignalR Connection Established");
            _logger.LogInformation("  UserId (from claim): {UserId}", userId ?? "NULL");
            _logger.LogInformation("  UserIdentifier (Context): {UserIdentifier}", userIdentifier ?? "NULL");
            _logger.LogInformation("  ConnectionId: {ConnectionId}", Context.ConnectionId);
            _logger.LogInformation("========================================");

            if (!string.IsNullOrEmpty(userId))
            {
                _connectionManager.AddConnection(userId, Context.ConnectionId);
                
                var allConnections = _connectionManager.GetConnections(userId);
                _logger.LogInformation("? User {UserId} registered with ConnectionId: {ConnectionId}", userId, Context.ConnectionId);
                _logger.LogInformation("  Total active connections for this user: {Count}", allConnections.Count);
            }
            else
            {
                _logger.LogWarning("?? SignalR connection without UserId! ConnectionId: {ConnectionId}", Context.ConnectionId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation("?? SignalR Disconnection:");
            _logger.LogInformation("  UserId: {UserId}", userId ?? "NULL");
            _logger.LogInformation("  ConnectionId: {ConnectionId}", Context.ConnectionId);

            if (exception != null)
            {
                _logger.LogError(exception, "  Disconnected with exception");
            }

            _connectionManager.RemoveConnection(Context.ConnectionId);
            _logger.LogInformation("? Connection {ConnectionId} removed from manager", Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Test method to verify SignalR connection is working
        /// Can be called from frontend: connection.invoke("SendTestMessage", "Hello")
        /// </summary>
        public async Task SendTestMessage(string message)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("?? Test message from user {UserId}: {Message}", userId, message);
            
            await Clients.Caller.SendAsync("ReceiveTestMessage", $"Echo from server: {message}");
            _logger.LogInformation("? Echo sent back to caller");
        }
    }
}
