using Korik.Domain;
using System;
using System.Text.Json.Serialization;

namespace Korik.Application
{
    /// <summary>
    /// DTO containing all information needed to display/restore a confirmation dialog
    /// Used when user clicks on notification from panel or when dialog needs to be shown
    /// </summary>
    public class PendingConfirmationDto
    {
        public int NotificationId { get; set; }
        public int BookingId { get; set; }
        
        [JsonConverter(typeof(JsonNumberEnumConverter<NotificationType>))]
        public NotificationType NotificationType { get; set; }
        
        public string Message { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime NotificationCreatedAt { get; set; }
        public bool IsRead { get; set; }
        
        // Booking details
        public string BookingStatus { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        
        // Confirmation details
        public DateTime? ConfirmationSentAt { get; set; }
        public DateTime? ConfirmationDeadline { get; set; }
        public int? RemainingSeconds { get; set; }
        public bool IsExpired { get; set; }
        public bool? CarOwnerConfirmed { get; set; }
        public bool? WorkshopOwnerConfirmed { get; set; }
        
        // Party information
        public string CarOwnerName { get; set; } = string.Empty;
        public string WorkshopName { get; set; } = string.Empty;
        
        // Indicates if dialog should still be shown (not confirmed, not expired, status still Confirmed)
        public bool CanStillConfirm { get; set; }
        
        // Service information
        public string ServiceName { get; set; } = string.Empty;
    }
}
