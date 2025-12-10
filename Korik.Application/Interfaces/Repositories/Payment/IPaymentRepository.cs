using Korik.Domain;

namespace Korik.Application
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task<Payment?> GetByStripePaymentIntentIdAsync(string paymentIntentId);
        Task<Payment?> GetByBookingIdAsync(int bookingId);
        Task<List<Payment>> GetPendingPayoutsAsync();
    }
}
