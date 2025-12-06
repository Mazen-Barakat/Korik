using AutoMapper;
using Korik.Application;
using Korik.Domain;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            INotificationRepository notificationRepository,
            IHubContext<NotificationHub> hubContext,
            IUserConnectionManager connectionManager,
            IMapper mapper,
            ILogger<NotificationService> logger)
        {
            _notificationRepository = notificationRepository;
            _hubContext = hubContext;
            _connectionManager = connectionManager;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResult<NotificationDto>> SendNotificationAsync(
            string senderId,
            string receiverId,
            string message,
            NotificationType type,
            int? bookingId = null,
            string? title = null,
            string? priority = null,
            DateTime? confirmationDeadline = null)
        {
            try
            {
                _logger.LogInformation("========================================");
                _logger.LogInformation("?? SENDING NOTIFICATION");
                _logger.LogInformation("  Receiver: {ReceiverId}", receiverId);
                _logger.LogInformation("  Type: {Type} (Value: {TypeValue})", type, (int)type);
                _logger.LogInformation("  BookingId: {BookingId}", bookingId);
                _logger.LogInformation("  Message: {Message}", message);
                _logger.LogInformation("  ConfirmationDeadline: {Deadline}", confirmationDeadline);
                _logger.LogInformation("========================================");

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
                    _logger.LogError("? Failed to save notification to database");
                    return ServiceResult<NotificationDto>.Fail("Failed to save notification to database.");
                }

                _logger.LogInformation("? Notification saved to database (ID: {NotificationId})", savedNotification.Id);

                var notificationDto = _mapper.Map<NotificationDto>(savedNotification);
                
                // Add extra fields for frontend
                notificationDto.Title = title ?? GetDefaultTitle(type);
                notificationDto.Priority = priority ?? "normal";
                
                // Set confirmation deadline - use provided value or calculate for confirmation requests
                if (type == NotificationType.AppointmentConfirmationRequest)
                {
                    notificationDto.ConfirmationDeadline = confirmationDeadline ?? DateTime.UtcNow.AddMinutes(15);
                }

                // 2. ?? CRITICAL: Send real-time notification via SignalR
                await SendViaSignalR(receiverId, notificationDto, type, bookingId);

                return ServiceResult<NotificationDto>.Created(notificationDto, "Notification sent successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Failed to send notification: {Message}", ex.Message);
                return ServiceResult<NotificationDto>.Fail($"Failed to send notification: {ex.Message}");
            }
        }

        private async Task SendViaSignalR(string receiverId, NotificationDto notificationDto, NotificationType type, int? bookingId)
        {
            try
            {
                _logger.LogInformation("?? Checking if user {ReceiverId} is connected...", receiverId);
                
                var isConnected = _connectionManager.IsUserConnected(receiverId);
                _logger.LogInformation("  IsUserConnected: {IsConnected}", isConnected);
                
                if (isConnected)
                {
                    var connectionIds = _connectionManager.GetConnections(receiverId);
                    _logger.LogInformation("? User {ReceiverId} is connected with {Count} connection(s)", receiverId, connectionIds.Count);

                    // Send to each individual connection
                    foreach (var connectionId in connectionIds)
                    {
                        _logger.LogInformation("  ?? Sending to connection: {ConnectionId}", connectionId);
                        await _hubContext.Clients.Client(connectionId)
                            .SendAsync("ReceiveNotification", notificationDto);
                        _logger.LogInformation("  ? Sent to connection: {ConnectionId}", connectionId);
                    }

                    // ?? ALSO send via User identifier (requires IUserIdProvider)
                    _logger.LogInformation("  ?? Sending via Clients.User({ReceiverId})...", receiverId);
                    await _hubContext.Clients.User(receiverId)
                        .SendAsync("ReceiveNotification", notificationDto);
                    _logger.LogInformation("  ? Sent via Clients.User()");

                    _logger.LogInformation("========================================");
                    _logger.LogInformation("?? REAL-TIME NOTIFICATION SENT SUCCESSFULLY");
                    _logger.LogInformation("  User: {ReceiverId}", receiverId);
                    _logger.LogInformation("  Type: {Type} ({TypeValue})", type, (int)type);
                    _logger.LogInformation("  BookingId: {BookingId}", bookingId);
                    _logger.LogInformation("========================================");
                }
                else
                {
                    _logger.LogWarning("?? User {ReceiverId} is OFFLINE. Notification saved to database only.", receiverId);
                    
                    // Still try to send via Clients.User in case the user connects soon
                    _logger.LogInformation("  ?? Attempting Clients.User() anyway...");
                    await _hubContext.Clients.User(receiverId)
                        .SendAsync("ReceiveNotification", notificationDto);
                }
            }
            catch (Exception signalREx)
            {
                _logger.LogError(signalREx, "? SignalR push failed: {Message}", signalREx.Message);
                // Notification is already saved in DB, so don't fail the whole operation
            }
        }

        public async Task<ServiceResult<bool>> SendBookingResponseChangedAsync(
            string receiverId,
            int bookingId,
            string bookingReference,
            int previousStatus,
            int newStatus,
            string changedBy)
        {
            try
            {
                _logger.LogInformation("========================================");
                _logger.LogInformation("?? SENDING BOOKING RESPONSE CHANGED EVENT");
                _logger.LogInformation("  Receiver: {ReceiverId}", receiverId);
                _logger.LogInformation("  BookingId: {BookingId}", bookingId);
                _logger.LogInformation("  Previous: {Previous} -> New: {New}", previousStatus, newStatus);
                _logger.LogInformation("========================================");

                var payload = new
                {
                    bookingId = bookingId,
                    bookingReference = bookingReference,
                    previousStatus = previousStatus,
                    newStatus = newStatus,
                    changedBy = changedBy,
                    changedAt = DateTime.UtcNow,
                    canChangeAgain = newStatus != 1 // Can't change if accepted
                };

                // Send via SignalR
                var isConnected = _connectionManager.IsUserConnected(receiverId);
                
                if (isConnected)
                {
                    var connectionIds = _connectionManager.GetConnections(receiverId);
                    foreach (var connectionId in connectionIds)
                    {
                        await _hubContext.Clients.Client(connectionId)
                            .SendAsync("BookingResponseChanged", payload);
                    }
                }

                // Also try via User identifier
                await _hubContext.Clients.User(receiverId)
                    .SendAsync("BookingResponseChanged", payload);

                _logger.LogInformation("? BookingResponseChanged event sent successfully");
                return ServiceResult<bool>.Ok(true, "Event sent successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Failed to send BookingResponseChanged event: {Message}", ex.Message);
                return ServiceResult<bool>.Fail($"Failed to send event: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> SendConfirmationStatusUpdateAsync(
            string receiverId,
            int bookingId,
            bool? carOwnerConfirmed,
            bool? workshopOwnerConfirmed,
            string newBookingStatus,
            bool shouldDismissDialog)
        {
            try
            {
                _logger.LogInformation("========================================");
                _logger.LogInformation("?? SENDING CONFIRMATION STATUS UPDATE");
                _logger.LogInformation("  Receiver: {ReceiverId}", receiverId);
                _logger.LogInformation("  BookingId: {BookingId}", bookingId);
                _logger.LogInformation("  CarOwnerConfirmed: {CarOwnerConfirmed}", carOwnerConfirmed);
                _logger.LogInformation("  WorkshopOwnerConfirmed: {WorkshopOwnerConfirmed}", workshopOwnerConfirmed);
                _logger.LogInformation("  NewBookingStatus: {NewBookingStatus}", newBookingStatus);
                _logger.LogInformation("  ShouldDismissDialog: {ShouldDismissDialog}", shouldDismissDialog);
                _logger.LogInformation("========================================");

                var payload = new
                {
                    bookingId = bookingId,
                    carOwnerConfirmed = carOwnerConfirmed,
                    workshopOwnerConfirmed = workshopOwnerConfirmed,
                    newBookingStatus = newBookingStatus,
                    shouldDismissDialog = shouldDismissDialog,
                    updatedAt = DateTime.UtcNow
                };

                // Send via SignalR to all user connections
                var isConnected = _connectionManager.IsUserConnected(receiverId);
                
                if (isConnected)
                {
                    var connectionIds = _connectionManager.GetConnections(receiverId);
                    foreach (var connectionId in connectionIds)
                    {
                        await _hubContext.Clients.Client(connectionId)
                            .SendAsync("ConfirmationStatusUpdate", payload);
                    }
                }

                // Also try via User identifier
                await _hubContext.Clients.User(receiverId)
                    .SendAsync("ConfirmationStatusUpdate", payload);

                _logger.LogInformation("? ConfirmationStatusUpdate event sent successfully");
                return ServiceResult<bool>.Ok(true, "Confirmation status update sent successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Failed to send ConfirmationStatusUpdate event: {Message}", ex.Message);
                return ServiceResult<bool>.Fail($"Failed to send event: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<PendingConfirmationDto>>> GetPendingConfirmationsAsync(string userId)
        {
            try
            {
                var notifications = await _notificationRepository.GetPendingConfirmationNotificationsAsync(userId);
                var now = DateTime.UtcNow;
                
                var pendingConfirmations = notifications
                    .Where(n => n.Booking != null)
                    .Select(n => MapToPendingConfirmationDto(n, now))
                    .ToList();

                return ServiceResult<IEnumerable<PendingConfirmationDto>>.Ok(pendingConfirmations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get pending confirmations for user {UserId}", userId);
                return ServiceResult<IEnumerable<PendingConfirmationDto>>.Fail($"Failed to get pending confirmations: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PendingConfirmationDto>> GetNotificationWithBookingDetailsAsync(int notificationId, string userId)
        {
            try
            {
                var notification = await _notificationRepository.GetNotificationWithBookingDetailsAsync(notificationId, userId);
                
                if (notification == null)
                {
                    return ServiceResult<PendingConfirmationDto>.Fail("Notification not found.");
                }

                if (notification.Booking == null)
                {
                    return ServiceResult<PendingConfirmationDto>.Fail("Notification has no associated booking.");
                }

                var now = DateTime.UtcNow;
                var pendingConfirmation = MapToPendingConfirmationDto(notification, now);

                return ServiceResult<PendingConfirmationDto>.Ok(pendingConfirmation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get notification {NotificationId} with booking details", notificationId);
                return ServiceResult<PendingConfirmationDto>.Fail($"Failed to get notification: {ex.Message}");
            }
        }

        private PendingConfirmationDto MapToPendingConfirmationDto(Notification notification, DateTime now)
        {
            var booking = notification.Booking!;
            var isExpired = booking.ConfirmationDeadline.HasValue && booking.ConfirmationDeadline.Value < now;
            var remainingSeconds = booking.ConfirmationDeadline.HasValue 
                ? (int)Math.Max(0, (booking.ConfirmationDeadline.Value - now).TotalSeconds)
                : (int?)null;

            // Can still confirm if: booking is Confirmed, not expired, and user hasn't confirmed yet
            var canStillConfirm = booking.Status == BookingStatus.Confirmed 
                                  && !isExpired 
                                  && booking.ConfirmationDeadline.HasValue;

            return new PendingConfirmationDto
            {
                NotificationId = notification.Id,
                BookingId = booking.Id,
                NotificationType = notification.Type,
                Message = notification.Message,
                Title = GetDefaultTitle(notification.Type),
                NotificationCreatedAt = notification.CreatedAt,
                IsRead = notification.IsRead,
                BookingStatus = booking.Status.ToString(),
                AppointmentDate = booking.AppointmentDate,
                ConfirmationSentAt = booking.ConfirmationSentAt,
                ConfirmationDeadline = booking.ConfirmationDeadline,
                RemainingSeconds = remainingSeconds,
                IsExpired = isExpired,
                CarOwnerConfirmed = booking.CarOwnerConfirmed,
                WorkshopOwnerConfirmed = booking.WorkshopOwnerConfirmed,
                CarOwnerName = booking.Car?.CarOwnerProfile != null 
                    ? $"{booking.Car.CarOwnerProfile.FirstName} {booking.Car.CarOwnerProfile.LastName}" 
                    : string.Empty,
                WorkshopName = booking.WorkShopProfile?.Name ?? string.Empty,
                CanStillConfirm = canStillConfirm,
                ServiceName = booking.WorkshopService?.Service?.Name ?? string.Empty
            };
        }

        private string GetDefaultTitle(NotificationType type)
        {
            return type switch
            {
                NotificationType.AppointmentConfirmationRequest => "Appointment Confirmation",
                NotificationType.BookingAccepted => "Booking Accepted",
                NotificationType.BookingRejected => "Booking Rejected",
                NotificationType.BookingCancelled => "Booking Cancelled",
                NotificationType.BookingCompleted => "Booking Completed",
                NotificationType.CarReadyForPickup => "Car Ready for Pickup",
                NotificationType.ResponseStatusChanged => "Response Status Changed",
                _ => "Notification"
            };
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
                _logger.LogError(ex, "Failed to get notifications for user {UserId}", userId);
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
                _logger.LogError(ex, "Failed to get unread count for user {UserId}", userId);
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
                _logger.LogError(ex, "Failed to mark notification {NotificationId} as read", notificationId);
                return ServiceResult<bool>.Fail($"Failed to mark notification as read: {ex.Message}");
            }
        }
    }
}