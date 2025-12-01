using AutoMapper;
using Korik.Application;
using Korik.Domain;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Infrastructure
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IUserConnectionManager _connectionManager;
        private readonly IMapper _mapper;

        public NotificationService(
            INotificationRepository notificationRepository,
         IHubContext<NotificationHub> hubContext,
            IUserConnectionManager connectionManager,
             IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _hubContext = hubContext;
            _connectionManager = connectionManager;
            _mapper = mapper;
        }

        public async Task<ServiceResult<NotificationDto>> SendNotificationAsync(
         string senderId,
            string receiverId,
          string message,
              NotificationType type,
             int? bookingId = null)
        {
            try
            {
                // 1. Create and save notification to database
                var notification = new Notification
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Message = message,
                    Type = type,
                    BookingId = bookingId,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                var savedNotification = await _notificationRepository.AddAsync(notification);

                if (savedNotification == null)
                {
                    return ServiceResult<NotificationDto>.Fail("Failed to save notification to database.");
                }

                var notificationDto = _mapper.Map<NotificationDto>(savedNotification);

                // 2. Try to send real-time notification via SignalR
                try
                {
                    if (_connectionManager.IsUserConnected(receiverId))
                    {
                        var connectionIds = _connectionManager.GetConnections(receiverId);

                        foreach (var connectionId in connectionIds)
                        {
                            await _hubContext.Clients.Client(connectionId)
                      .SendAsync("ReceiveNotification", notificationDto);
                        }

                        Console.WriteLine($"Real-time notification sent to user {receiverId}");
                    }
                    else
                    {
                        Console.WriteLine($"User {receiverId} is offline. Notification saved to database.");
                    }
                }
                catch (Exception signalREx)
                {
                    // SignalR push failed, but notification is already saved in DB
                    Console.WriteLine($"SignalR push failed: {signalREx.Message}, but notification is saved.");
                }

                return ServiceResult<NotificationDto>.Created(notificationDto, "Notification sent successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<NotificationDto>.Fail($"Failed to send notification: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<NotificationDto>>> GetUserNotificationsAsync(string userId)
        {
            try
            {
                var notifications = await _notificationRepository.GetNotificationsByUserIdAsync(userId);
                var notificationDtos = _mapper.Map<IEnumerable<NotificationDto>>(notifications);

                return ServiceResult<IEnumerable<NotificationDto>>.Ok(notificationDtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<NotificationDto>>.Fail($"Failed to get notifications: {ex.Message}");
            }
        }

        public async Task<ServiceResult<int>> GetUnreadCountAsync(string userId)
        {
            try
            {
                var count = await _notificationRepository.GetUnreadCountByUserIdAsync(userId);
                return ServiceResult<int>.Ok(count);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail($"Failed to get unread count: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> MarkAsReadAsync(int notificationId)
        {
            try
            {
                var result = await _notificationRepository.MarkAsReadAsync(notificationId);

                if (!result)
                {
                    return ServiceResult<bool>.Fail("Notification not found.");
                }

                return ServiceResult<bool>.Ok(true, "Notification marked as read.");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail($"Failed to mark notification as read: {ex.Message}");
            }
        }
    }
}