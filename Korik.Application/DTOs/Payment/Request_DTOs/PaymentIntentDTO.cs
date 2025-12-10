namespace Korik.Application
{
    public class PaymentIntentDTO
    {
        public string ClientSecret { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal CommissionAmount { get; set; }
        public decimal WorkshopAmount { get; set; }
    }
}
