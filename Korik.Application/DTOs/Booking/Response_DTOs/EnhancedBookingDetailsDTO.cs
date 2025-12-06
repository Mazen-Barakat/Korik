using Korik.Domain;
using System;
using System.Collections.Generic;

namespace Korik.Application
{
    /// <summary>
    /// Enhanced booking details DTO with complete customer, vehicle, and service information
    /// </summary>
    public class EnhancedBookingDetailsDTO
    {
        public int Id { get; set; }
        public int BookingId => Id;
        public string BookingReference { get; set; } = string.Empty;

        // Customer Info
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string? CustomerPhoto { get; set; }

        // Vehicle Info
        public string VehicleInfo { get; set; } = string.Empty;
        public string? VehicleMake { get; set; }
        public string? VehicleModel { get; set; }
        public int? VehicleYear { get; set; }
        public string? VehiclePlateNumber { get; set; }

        // Service Info
        public string ServiceType { get; set; } = string.Empty;
        public List<string> ServiceList { get; set; } = new();
        public int? EstimatedDuration { get; set; }
        public decimal? EstimatedCost { get; set; }

        // Appointment Info
        public DateTime ExactAppointmentTime { get; set; }
        public DateTime CreatedAt { get; set; }

        // Workshop Info
        public string WorkshopName { get; set; } = string.Empty;
        public string? WorkshopAddress { get; set; }
        public string? WorkshopPhone { get; set; }

        // Response Status
        public int ResponseStatus { get; set; } // 0=Pending, 1=Accepted, 2=Declined
        public bool CanChangeResponse { get; set; }
        public DateTime? LastResponseChangedAt { get; set; }
        public string? ResponseChangedBy { get; set; }

        // Confirmation Status
        public bool? CarOwnerConfirmed { get; set; }
        public bool? WorkshopConfirmed { get; set; }
        public bool BothConfirmed => CarOwnerConfirmed == true && WorkshopConfirmed == true;

        // Notification Info
        public int NotificationType { get; set; }
        public string Priority { get; set; } = "normal";
        public string Status { get; set; } = string.Empty;
    }
}
