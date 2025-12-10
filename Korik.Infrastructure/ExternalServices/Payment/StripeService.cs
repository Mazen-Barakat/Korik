using Korik.Application;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Korik.Infrastructure
{
    public class StripeService : IStripeService
    {
        private readonly string _secretKey;

        public StripeService(IConfiguration configuration)
        {
            _secretKey = configuration["Stripe:SecretKey"];
            StripeConfiguration.ApiKey = _secretKey;
        }

        public async Task<ServiceResult<StripePaymentIntentResult>> CreatePaymentIntentAsync(
            decimal amount,
            string currency,
            Dictionary<string, string> metadata)
        {
            try
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(amount * 100), // Stripe uses cents
                    Currency = currency.ToLower(),
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true,
                    },
                    Metadata = metadata,
                    CaptureMethod = "automatic"
                };

                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options);

                var result = new StripePaymentIntentResult
                {
                    Id = paymentIntent.Id,
                    ClientSecret = paymentIntent.ClientSecret,
                    Status = paymentIntent.Status,
                    Amount = paymentIntent.Amount,
                    Currency = paymentIntent.Currency
                };

                return ServiceResult<StripePaymentIntentResult>.Ok(result, "Payment intent created successfully.");
            }
            catch (StripeException ex)
            {
                return ServiceResult<StripePaymentIntentResult>.Fail($"Stripe error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return ServiceResult<StripePaymentIntentResult>.Fail($"Error creating payment intent: {ex.Message}");
            }
        }

        public async Task<ServiceResult<StripePaymentIntentResult>> ConfirmPaymentIntentAsync(string paymentIntentId)
        {
            try
            {
                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(paymentIntentId);

                var result = new StripePaymentIntentResult
                {
                    Id = paymentIntent.Id,
                    ClientSecret = paymentIntent.ClientSecret,
                    Status = paymentIntent.Status,
                    Amount = paymentIntent.Amount,
                    Currency = paymentIntent.Currency
                };

                return ServiceResult<StripePaymentIntentResult>.Ok(result, "Payment intent retrieved successfully.");
            }
            catch (StripeException ex)
            {
                return ServiceResult<StripePaymentIntentResult>.Fail($"Stripe error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return ServiceResult<StripePaymentIntentResult>.Fail($"Error confirming payment: {ex.Message}");
            }
        }

        public async Task<ServiceResult<StripeRefundResult>> RefundPaymentAsync(string paymentIntentId, decimal? amount = null)
        {
            try
            {
                var options = new RefundCreateOptions
                {
                    PaymentIntent = paymentIntentId
                };

                if (amount.HasValue)
                {
                    options.Amount = (long)(amount.Value * 100);
                }

                var service = new RefundService();
                var refund = await service.CreateAsync(options);

                var result = new StripeRefundResult
                {
                    Id = refund.Id,
                    Status = refund.Status,
                    Amount = refund.Amount,
                    PaymentIntentId = refund.PaymentIntentId
                };

                return ServiceResult<StripeRefundResult>.Ok(result, "Refund processed successfully.");
            }
            catch (StripeException ex)
            {
                return ServiceResult<StripeRefundResult>.Fail($"Stripe error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return ServiceResult<StripeRefundResult>.Fail($"Error processing refund: {ex.Message}");
            }
        }
    }
}
