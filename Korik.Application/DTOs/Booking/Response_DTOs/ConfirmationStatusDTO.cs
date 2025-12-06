namespace Korik.Application
{
    public class ConfirmationStatusDTO
    {
        public int BookingId { get; set; }
        public bool? CarOwnerConfirmed { get; set; }
        public bool? WorkshopConfirmed { get; set; }
        public bool BothConfirmed => CarOwnerConfirmed == true && WorkshopConfirmed == true;
        public string Status { get; set; } = string.Empty;
        public DateTime? ConfirmationSentAt { get; set; }
        public DateTime? ConfirmationDeadline { get; set; }
        public int? RemainingSeconds { get; set; }
    }
}
