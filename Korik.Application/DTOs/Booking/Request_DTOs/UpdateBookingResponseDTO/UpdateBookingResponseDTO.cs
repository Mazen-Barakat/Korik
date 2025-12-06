using System;

namespace Korik.Application
{
    /// <summary>
    /// Response status update DTO
    /// </summary>
    public class UpdateBookingResponseDTO
    {
        public int ResponseStatus { get; set; } // 0=Pending, 1=Accepted, 2=Declined
        public string ChangedBy { get; set; } = string.Empty; // "workshop" or "customer"
    }

    /// <summary>
    /// Response status update result DTO
    /// </summary>
    public class BookingResponseResultDTO
    {
        public int BookingId { get; set; }
        public string BookingReference { get; set; } = string.Empty;
        public int PreviousStatus { get; set; }
        public int NewStatus { get; set; }
        public bool CanChangeAgain { get; set; }
        public DateTime? LastResponseChangedAt { get; set; }
        public string? ResponseChangedBy { get; set; }
    }
}
