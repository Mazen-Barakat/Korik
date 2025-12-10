namespace Korik.Application
{
    public class MarkAsPaidOutDTO
    {
        public int PaymentId { get; set; }
        public string PayoutMethod { get; set; } // e.g., "BankTransfer", "Cash", "Check"
        public string? PayoutReference { get; set; } // Bank reference or transaction ID
        public string? Notes { get; set; }
    }
}
