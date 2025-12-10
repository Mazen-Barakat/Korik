using Korik.Application;
using Korik.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Stripe;

namespace Korik.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _configuration;

        public PaymentController(IPaymentService paymentService, IConfiguration configuration)
        {
            _paymentService = paymentService;
            _configuration = configuration;
        }

        [HttpPost("create-payment-intent")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Create payment intent for booking",
            Description = "Creates a Stripe payment intent for car owner to pay for a booking. Returns client secret for frontend integration. Automatically calculates 12% commission."
        )]
        public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentDTO dto)
        {
            var result = await _paymentService.CreatePaymentIntentAsync(dto);
            return ApiResponse.FromResult(this, result);
        }

        [HttpPost("webhook")]
        [SwaggerOperation(
            Summary = "Stripe webhook endpoint",
            Description = "Handles Stripe webhook events (payment success, failure, etc.). Called automatically by Stripe servers."
        )]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            
            try
            {
                // Validate required Stripe-Signature header exists
                if (!Request.Headers.ContainsKey("Stripe-Signature"))
                {
                    return BadRequest(new { error = "Missing Stripe-Signature header. This endpoint must be called by Stripe webhooks." });
                }

                var signature = Request.Headers["Stripe-Signature"].ToString();
                if (string.IsNullOrEmpty(signature))
                {
                    return BadRequest(new { error = "Empty Stripe-Signature header" });
                }

                var webhookSecret = _configuration["Stripe:WebhookSecret"];
                if (string.IsNullOrEmpty(webhookSecret))
                {
                    return StatusCode(500, new { error = "Webhook secret not configured" });
                }

                // ? FIX: Suppress API version mismatch error
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    signature,
                    webhookSecret,
                    throwOnApiVersionMismatch: false
                );

                // Log the event for debugging
                Console.WriteLine($"[Webhook] Received event: {stripeEvent.Type} with ID: {stripeEvent.Id}");

                // Handle different event types
                switch (stripeEvent.Type)
                {
                    case "payment_intent.succeeded":
                        {
                            var successIntent = stripeEvent.Data.Object as PaymentIntent;
                            if (successIntent != null)
                            {
                                Console.WriteLine($"[Webhook] Processing payment success for PaymentIntent: {successIntent.Id}");
                                await _paymentService.HandlePaymentSuccessAsync(successIntent.Id);
                            }
                            break;
                        }

                    case "payment_intent.payment_failed":
                        {
                            var failedIntent = stripeEvent.Data.Object as PaymentIntent;
                            if (failedIntent != null)
                            {
                                Console.WriteLine($"[Webhook] Processing payment failure for PaymentIntent: {failedIntent.Id}");
                                await _paymentService.HandlePaymentFailureAsync(failedIntent.Id);
                            }
                            break;
                        }

                    case "payment_intent.canceled":
                        {
                            var canceledIntent = stripeEvent.Data.Object as PaymentIntent;
                            if (canceledIntent != null)
                            {
                                Console.WriteLine($"[Webhook] Processing payment cancelation for PaymentIntent: {canceledIntent.Id}");
                                await _paymentService.HandlePaymentCanceledAsync(canceledIntent.Id);
                            }
                            break;
                        }

                    case "charge.refunded":
                        {
                            var refund = stripeEvent.Data.Object as Charge;
                            if (refund != null)
                            {
                                Console.WriteLine($"[Webhook] Refund completed for Charge: {refund.Id}");
                                // Refund is already handled in RefundPaymentAsync endpoint
                                // This webhook just confirms it completed successfully
                            }
                            break;
                        }

                    case "charge.dispute.created":
                        {
                            var dispute = stripeEvent.Data.Object as Dispute;
                            if (dispute != null)
                            {
                                Console.WriteLine($"[Webhook] ?? DISPUTE CREATED for Charge: {dispute.ChargeId}");
                                // TODO: Implement dispute handling (notify admin, flag payment, etc.)
                                // For now, just log it for manual review
                            }
                            break;
                        }

                    default:
                        Console.WriteLine($"[Webhook] Unhandled event type: {stripeEvent.Type}");
                        break;
                }

                return Ok();
            }
            catch (StripeException ex)
            {
                Console.WriteLine($"[Webhook Error] Stripe exception: {ex.Message}");
                return BadRequest(new { error = $"Stripe webhook error: {ex.Message}" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Webhook Error] General exception: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("refund/{bookingId}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(
            Summary = "Refund payment for booking",
            Description = "Processes a full refund for a booking payment and cancels the booking (Admin only)"
        )]
        public async Task<IActionResult> RefundPayment([FromRoute] int bookingId)
        {
            var result = await _paymentService.RefundPaymentAsync(bookingId);
            return ApiResponse.FromResult(this, result);
        }

        [HttpGet("booking/{bookingId}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Get payment details for a booking",
            Description = "Returns payment information including amounts and status for a specific booking"
        )]
        public async Task<IActionResult> GetPaymentByBookingId([FromRoute] int bookingId)
        {
            var result = await _paymentService.GetAllAsync();
            if (!result.Success)
                return ApiResponse.FromResult(this, result);

            var payment = result.Data.FirstOrDefault(p => p.BookingId == bookingId);
            if (payment == null)
                return ApiResponse.FromResult(this, ServiceResult<Payment>.Fail("Payment not found for this booking."));

            return ApiResponse.FromResult(this, ServiceResult<Payment>.Ok(payment));
        }

        [HttpGet("pending-payouts")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(
            Summary = "Get all pending workshop payouts",
            Description = "Returns list of completed bookings that have been paid but workshop hasn't been paid out yet (Admin only)"
        )]
        public async Task<IActionResult> GetPendingPayouts()
        {
            var result = await _paymentService.GetPendingPayoutsAsync();
            
            if (!result.Success)
                return ApiResponse.FromResult(this, result);

            // Map to DTO for better frontend consumption
            var pendingPayouts = result.Data.Select(p => new PendingPayoutDTO
            {
                PaymentId = p.Id,
                BookingId = p.BookingId,
                WorkshopAmount = p.WorkshopAmount,
                CommissionAmount = p.CommissionAmount,
                TotalAmount = p.TotalAmount,
                PaidAt = p.PaidAt ?? DateTime.UtcNow,
                DaysPending = p.PaidAt.HasValue ? (DateTime.UtcNow - p.PaidAt.Value).Days : 0,
                WorkshopId = p.Booking.WorkShopProfileId,
                WorkshopName = p.Booking.WorkShopProfile.Name,
                WorkshopPhone = p.Booking.WorkShopProfile.PhoneNumber,
                ServiceName = p.Booking.WorkshopService.Service.Name,
                CarOwnerName = p.Booking.Car.CarOwnerProfile.ApplicationUser.UserName ?? "Unknown",
                AppointmentDate = p.Booking.AppointmentDate,
                CompletedDate = p.Booking.CreatedAt
            }).ToList();

            return ApiResponse.FromResult(this, ServiceResult<IEnumerable<PendingPayoutDTO>>.Ok(pendingPayouts));
        }

        [HttpPost("mark-paid-out")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(
            Summary = "Mark payment as paid out to workshop",
            Description = "Records that workshop has received their payment manually (Admin only)"
        )]
        public async Task<IActionResult> MarkAsPaidOut([FromBody] MarkAsPaidOutDTO dto)
        {
            var result = await _paymentService.MarkAsPaidOutAsync(
                dto.PaymentId,
                dto.PayoutMethod,
                dto.PayoutReference,
                dto.Notes
            );
            
            return ApiResponse.FromResult(this, result);
        }

        [HttpGet("payout-history")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(
            Summary = "Get payout history",
            Description = "Returns list of all payments that have been paid out to workshops (Admin only)"
        )]
        public async Task<IActionResult> GetPayoutHistory()
        {
            var result = await _paymentService.GetAllAsync();
            
            if (!result.Success)
                return ApiResponse.FromResult(this, result);

            var paidOutPayments = result.Data
                .Where(p => p.IsPaidOut)
                .OrderByDescending(p => p.PayoutDate)
                .ToList();

            return ApiResponse.FromResult(this, ServiceResult<IEnumerable<Payment>>.Ok(paidOutPayments));
        }
    }
}
