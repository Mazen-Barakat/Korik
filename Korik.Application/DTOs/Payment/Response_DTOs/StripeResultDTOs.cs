namespace Korik.Application
{
    public class StripePaymentIntentResult
    {
        public string Id { get; set; }
        public string ClientSecret { get; set; }
        public string Status { get; set; }
        public long Amount { get; set; }
        public string Currency { get; set; }
    }

    public class StripeRefundResult
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public long Amount { get; set; }
        public string PaymentIntentId { get; set; }
    }
}
