using System.ComponentModel.DataAnnotations.Schema;

namespace Korik.Domain
{
    public enum StripePaymentStatus
    {
        Pending,
        Succeeded,
        Failed,
        Refunded
    }

    public class Payment : BaseEntity
    {
        [ForeignKey("Booking")]
        public int BookingId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal CommissionAmount { get; set; }
        public decimal WorkshopAmount { get; set; }
        public decimal CommissionRate { get; set; } = 0.12m; // 12%
        
        public StripePaymentStatus StripePaymentStatus { get; set; } = StripePaymentStatus.Pending;
        
        // Stripe specific fields
        public string StripePaymentIntentId { get; set; }
        
        // Payout tracking fields
        public bool IsPaidOut { get; set; } = false;
        public DateTime? PayoutDate { get; set; }
        public string? PayoutMethod { get; set; } // e.g., "BankTransfer", "Cash", "Check"
        public string? PayoutReference { get; set; } // Bank reference or transaction ID
        public string? PayoutNotes { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }

        #region Payment M---1 Booking
        public virtual Booking Booking { get; set; }
        #endregion
    }
}
