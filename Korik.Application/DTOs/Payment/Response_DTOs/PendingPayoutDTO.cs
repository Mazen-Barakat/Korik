namespace Korik.Application
{
    public class PendingPayoutDTO
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public decimal WorkshopAmount { get; set; }
        public decimal CommissionAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime PaidAt { get; set; }
        public int DaysPending { get; set; }
        
        // Workshop Info
        public int WorkshopId { get; set; }
        public string WorkshopName { get; set; }
        public string WorkshopPhone { get; set; }
        
        // Service Info
        public string ServiceName { get; set; }
        
        // Car Owner Info
        public string CarOwnerName { get; set; }
        
        // Booking Info
        public DateTime AppointmentDate { get; set; }
        public DateTime CompletedDate { get; set; }
    }
}
