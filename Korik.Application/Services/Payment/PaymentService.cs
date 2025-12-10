using Korik.Domain;
using Microsoft.EntityFrameworkCore;

namespace Korik.Application
{
    public class PaymentService : GenericService<Payment>, IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IStripeService _stripeService;
        private const decimal COMMISSION_RATE = 0.12m; // 12%

        public PaymentService(
            IPaymentRepository paymentRepository,
            IBookingRepository bookingRepository,
            IStripeService stripeService) : base(paymentRepository)
        {
            _paymentRepository = paymentRepository;
            _bookingRepository = bookingRepository;
            _stripeService = stripeService;
        }

        public async Task<ServiceResult<string>> CreatePaymentIntentAsync(CreatePaymentDTO model)
        {
            try
            {
                // Get booking with service details
                var booking = await _bookingRepository.GetAllAsync()
                    .Include(b => b.WorkshopService)
                    .Include(b => b.WorkShopProfile)
                    .FirstOrDefaultAsync(b => b.Id == model.BookingId);

                if (booking == null)
                    return ServiceResult<string>.Fail("Booking not found.");

                if (booking.PaymentStatus == PaymentStatus.Paid)
                    return ServiceResult<string>.Fail("Booking already paid.");

                // Check if payment already exists
                var existingPayment = await _paymentRepository.GetByBookingIdAsync(model.BookingId);
                if (existingPayment != null)
                {
                    // Return existing client secret if payment is still pending
                    if (existingPayment.StripePaymentStatus == StripePaymentStatus.Pending)
                    {
                        var existingIntent = await _stripeService.ConfirmPaymentIntentAsync(existingPayment.StripePaymentIntentId);
                        if (existingIntent.Success && existingIntent.Data != null)
                        {
                            return ServiceResult<string>.Ok(existingIntent.Data.ClientSecret, "Existing payment intent retrieved.");
                        }
                    }
                    return ServiceResult<string>.Fail("Payment already exists for this booking.");
                }

                // Calculate amounts
                var totalAmount = model.TotalAmount;
                var commissionAmount = totalAmount * COMMISSION_RATE;
                var workshopAmount = totalAmount - commissionAmount;

                // Create Stripe payment intent
                var metadata = new Dictionary<string, string>
                {
                    { "booking_id", model.BookingId.ToString() },
                    { "workshop_id", booking.WorkShopProfileId.ToString() },
                    { "commission_rate", COMMISSION_RATE.ToString("F2") }
                };

                var stripeResult = await _stripeService.CreatePaymentIntentAsync(
                    totalAmount, 
                    "usd", 
                    metadata);

                if (!stripeResult.Success || stripeResult.Data == null)
                    return ServiceResult<string>.Fail(stripeResult.Message);

                // Create payment record
                var payment = new Payment
                {
                    BookingId = model.BookingId,
                    TotalAmount = totalAmount,
                    CommissionAmount = commissionAmount,
                    WorkshopAmount = workshopAmount,
                    CommissionRate = COMMISSION_RATE,
                    StripePaymentStatus = StripePaymentStatus.Pending,
                    StripePaymentIntentId = stripeResult.Data.Id
                };

                var createResult = await CreateAsync(payment);
                if (!createResult.Success)
                    return ServiceResult<string>.Fail(createResult.Message);

                return ServiceResult<string>.Ok(
                    stripeResult.Data.ClientSecret, 
                    "Payment intent created successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.Fail($"Error creating payment: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Payment>> HandlePaymentSuccessAsync(string paymentIntentId)
        {
            try
            {
                Console.WriteLine($"[PaymentService] Starting HandlePaymentSuccessAsync for PaymentIntent: {paymentIntentId}");
                
                var payment = await _paymentRepository.GetByStripePaymentIntentIdAsync(paymentIntentId);
                if (payment == null)
                {
                    Console.WriteLine($"[PaymentService] ERROR: Payment not found for PaymentIntent: {paymentIntentId}");
                    return ServiceResult<Payment>.Fail("Payment not found.");
                }

                Console.WriteLine($"[PaymentService] Found payment ID: {payment.Id}, Current status: {payment.StripePaymentStatus}");

                if (payment.StripePaymentStatus == StripePaymentStatus.Succeeded)
                {
                    Console.WriteLine($"[PaymentService] Payment already processed for ID: {payment.Id}");
                    return ServiceResult<Payment>.Ok(payment, "Payment already processed.");
                }

                // Confirm payment with Stripe
                Console.WriteLine($"[PaymentService] Confirming payment with Stripe...");
                var stripeResult = await _stripeService.ConfirmPaymentIntentAsync(paymentIntentId);
                if (!stripeResult.Success || stripeResult.Data?.Status != "succeeded")
                {
                    Console.WriteLine($"[PaymentService] ERROR: Payment confirmation failed: {stripeResult.Message}");
                    return ServiceResult<Payment>.Fail("Payment confirmation failed.");
                }

                Console.WriteLine($"[PaymentService] Stripe confirmation successful. Status: {stripeResult.Data.Status}");

                // Update payment status
                payment.StripePaymentStatus = StripePaymentStatus.Succeeded;
                payment.PaidAt = DateTime.UtcNow;

                // Update booking
                var booking = payment.Booking;
                Console.WriteLine($"[PaymentService] Updating booking ID: {payment.BookingId}");
                booking.PaymentStatus = PaymentStatus.Paid;
                booking.Status = BookingStatus.Completed;
                booking.PaidAmount = payment.TotalAmount;

                await _paymentRepository.UpdateAsync(payment);
                await _bookingRepository.UpdateAsync(booking);

                Console.WriteLine($"[PaymentService] ? Payment processed successfully. Payment ID: {payment.Id}, Booking ID: {payment.BookingId}");

                return ServiceResult<Payment>.Ok(payment, "Payment processed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PaymentService] EXCEPTION in HandlePaymentSuccessAsync: {ex.Message}");
                Console.WriteLine($"[PaymentService] Stack trace: {ex.StackTrace}");
                return ServiceResult<Payment>.Fail($"Error processing payment: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Payment>> HandlePaymentFailureAsync(string paymentIntentId)
        {
            try
            {
                Console.WriteLine($"[PaymentService] Starting HandlePaymentFailureAsync for PaymentIntent: {paymentIntentId}");
                
                var payment = await _paymentRepository.GetByStripePaymentIntentIdAsync(paymentIntentId);
                if (payment == null)
                {
                    Console.WriteLine($"[PaymentService] ERROR: Payment not found for PaymentIntent: {paymentIntentId}");
                    return ServiceResult<Payment>.Fail("Payment not found.");
                }

                Console.WriteLine($"[PaymentService] Found payment ID: {payment.Id}, Current status: {payment.StripePaymentStatus}");

                if (payment.StripePaymentStatus == StripePaymentStatus.Failed)
                {
                    Console.WriteLine($"[PaymentService] Payment already marked as failed for ID: {payment.Id}");
                    return ServiceResult<Payment>.Ok(payment, "Payment already marked as failed.");
                }

                // Update payment status
                payment.StripePaymentStatus = StripePaymentStatus.Failed;

                // Update booking - keep it in pending state so user can retry payment
                var booking = payment.Booking;
                Console.WriteLine($"[PaymentService] Updating booking ID: {payment.BookingId} to Pending (allow retry)");
                booking.PaymentStatus = PaymentStatus.Unpaid;
                booking.Status = BookingStatus.Pending;
                booking.PaidAmount = null;

                await _paymentRepository.UpdateAsync(payment);
                await _bookingRepository.UpdateAsync(booking);

                Console.WriteLine($"[PaymentService] ? Payment marked as failed. Payment ID: {payment.Id}, Booking ID: {payment.BookingId}");

                return ServiceResult<Payment>.Ok(payment, "Payment marked as failed. Customer can retry payment.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PaymentService] EXCEPTION in HandlePaymentFailureAsync: {ex.Message}");
                Console.WriteLine($"[PaymentService] Stack trace: {ex.StackTrace}");
                return ServiceResult<Payment>.Fail($"Error handling payment failure: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Payment>> HandlePaymentCanceledAsync(string paymentIntentId)
        {
            try
            {
                Console.WriteLine($"[PaymentService] Starting HandlePaymentCanceledAsync for PaymentIntent: {paymentIntentId}");
                
                var payment = await _paymentRepository.GetByStripePaymentIntentIdAsync(paymentIntentId);
                if (payment == null)
                {
                    Console.WriteLine($"[PaymentService] ERROR: Payment not found for PaymentIntent: {paymentIntentId}");
                    return ServiceResult<Payment>.Fail("Payment not found.");
                }

                Console.WriteLine($"[PaymentService] Found payment ID: {payment.Id}, Current status: {payment.StripePaymentStatus}");

                if (payment.StripePaymentStatus == StripePaymentStatus.Canceled)
                {
                    Console.WriteLine($"[PaymentService] Payment already marked as canceled for ID: {payment.Id}");
                    return ServiceResult<Payment>.Ok(payment, "Payment already marked as canceled.");
                }

                // Update payment status
                payment.StripePaymentStatus = StripePaymentStatus.Canceled;

                // Update booking - cancel the booking since payment was explicitly canceled
                var booking = payment.Booking;
                Console.WriteLine($"[PaymentService] Canceling booking ID: {payment.BookingId}");
                booking.PaymentStatus = PaymentStatus.Unpaid;
                booking.Status = BookingStatus.Cancelled;
                booking.PaidAmount = null;

                await _paymentRepository.UpdateAsync(payment);
                await _bookingRepository.UpdateAsync(booking);

                Console.WriteLine($"[PaymentService] ?? Payment canceled. Payment ID: {payment.Id}, Booking ID: {payment.BookingId}");

                return ServiceResult<Payment>.Ok(payment, "Payment marked as canceled. Booking has been cancelled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PaymentService] EXCEPTION in HandlePaymentCanceledAsync: {ex.Message}");
                Console.WriteLine($"[PaymentService] Stack trace: {ex.StackTrace}");
                return ServiceResult<Payment>.Fail($"Error handling payment cancelation: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> RefundPaymentAsync(int bookingId)
        {
            try
            {
                var payment = await _paymentRepository.GetByBookingIdAsync(bookingId);
                if (payment == null)
                    return ServiceResult<bool>.Fail("Payment not found.");

                if (payment.StripePaymentStatus != StripePaymentStatus.Succeeded)
                    return ServiceResult<bool>.Fail("Payment not eligible for refund.");

                if (payment.IsPaidOut)
                    return ServiceResult<bool>.Fail("Cannot refund - workshop has already been paid out.");

                var refundResult = await _stripeService.RefundPaymentAsync(payment.StripePaymentIntentId);
                if (!refundResult.Success)
                    return ServiceResult<bool>.Fail(refundResult.Message);

                payment.StripePaymentStatus = StripePaymentStatus.Refunded;
                await _paymentRepository.UpdateAsync(payment);

                // Update booking
                var booking = payment.Booking;
                booking.PaymentStatus = PaymentStatus.Unpaid;
                booking.PaidAmount = null;
                booking.Status = BookingStatus.Cancelled;
                await _bookingRepository.UpdateAsync(booking);

                return ServiceResult<bool>.Ok(true, "Refund processed successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail($"Error processing refund: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<Payment>>> GetPendingPayoutsAsync()
        {
            try
            {
                var pendingPayouts = await _paymentRepository.GetPendingPayoutsAsync();
                
                return ServiceResult<List<Payment>>.Ok(
                    pendingPayouts, 
                    $"Retrieved {pendingPayouts.Count} pending payouts.");
            }
            catch (Exception ex)
            {
                return ServiceResult<List<Payment>>.Fail($"Error retrieving pending payouts: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Payment>> MarkAsPaidOutAsync(
            int paymentId, 
            string payoutMethod, 
            string? payoutReference = null, 
            string? notes = null)
        {
            try
            {
                var paymentResult = await GetByIdAsync(paymentId);
                if (!paymentResult.Success || paymentResult.Data == null)
                    return ServiceResult<Payment>.Fail("Payment not found.");

                var payment = paymentResult.Data;

                if (payment.StripePaymentStatus != StripePaymentStatus.Succeeded)
                    return ServiceResult<Payment>.Fail("Payment must be in Succeeded status to be paid out.");

                if (payment.Booking.Status != BookingStatus.Completed)
                    return ServiceResult<Payment>.Fail("Booking must be completed before payout.");

                if (payment.IsPaidOut)
                    return ServiceResult<Payment>.Fail("Payment has already been paid out.");

                // Mark as paid out
                payment.IsPaidOut = true;
                payment.PayoutDate = DateTime.UtcNow;
                payment.PayoutMethod = payoutMethod;
                payment.PayoutReference = payoutReference;
                payment.PayoutNotes = notes;

                await _paymentRepository.UpdateAsync(payment);

                return ServiceResult<Payment>.Ok(
                    payment, 
                    $"Payment marked as paid out. Workshop received ${payment.WorkshopAmount:F2}");
            }
            catch (Exception ex)
            {
                return ServiceResult<Payment>.Fail($"Error marking payment as paid out: {ex.Message}");
            }
        }
    }
}
