namespace Korik.Application
{
    public class ConfirmAppointmentDTO
    {
        public int BookingId { get; set; }
        public bool IsConfirmed { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;
    }
}
