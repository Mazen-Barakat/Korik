using Korik.Domain;

namespace Korik.Application
{
    public interface IPaymentService : IGenericService<Payment>
    {
        Task<ServiceResult<string>> CreatePaymentIntentAsync(CreatePaymentDTO dto);
        Task<ServiceResult<Payment>> HandlePaymentSuccessAsync(string paymentIntentId);
        Task<ServiceResult<bool>> RefundPaymentAsync(int bookingId);
        Task<ServiceResult<List<Payment>>> GetPendingPayoutsAsync();
        Task<ServiceResult<Payment>> MarkAsPaidOutAsync(int paymentId, string payoutMethod, string? payoutReference = null, string? notes = null);
    }
}
