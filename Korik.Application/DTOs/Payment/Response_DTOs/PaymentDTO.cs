using Korik.Domain;

namespace Korik.Application
{
    public class PaymentDTO
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal CommissionAmount { get; set; }
        public decimal WorkshopAmount { get; set; }
        public StripePaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        
        // Payout information
        public bool IsPaidOut { get; set; }
        public DateTime? PayoutDate { get; set; }
        public string? PayoutMethod { get; set; }
        public string? PayoutReference { get; set; }
    }
}
