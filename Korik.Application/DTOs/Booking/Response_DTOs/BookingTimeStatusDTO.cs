using System;

namespace Korik.Application
{
    /// <summary>
    /// Time status DTO for precise timing information
    /// </summary>
    public class BookingTimeStatusDTO
    {
        public int BookingId { get; set; }
        public string BookingReference { get; set; } = string.Empty;
        public DateTime ExactAppointmentTime { get; set; }
        public DateTime CurrentTime { get; set; }
        public bool HasArrived { get; set; }
        public int SecondsUntilArrival { get; set; }
        public bool CanStillChangeResponse { get; set; }
        public int ResponseStatus { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
