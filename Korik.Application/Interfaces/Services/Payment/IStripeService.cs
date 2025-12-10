namespace Korik.Application
{
    public interface IStripeService
    {
        Task<ServiceResult<StripePaymentIntentResult>> CreatePaymentIntentAsync(decimal amount, string currency, Dictionary<string, string> metadata);
        Task<ServiceResult<StripePaymentIntentResult>> ConfirmPaymentIntentAsync(string paymentIntentId);
        Task<ServiceResult<StripeRefundResult>> RefundPaymentAsync(string paymentIntentId, decimal? amount = null);
    }
}
