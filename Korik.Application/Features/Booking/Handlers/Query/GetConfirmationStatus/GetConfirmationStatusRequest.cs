using Korik.Domain;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record GetConfirmationStatusRequest(int BookingId, string ApplicationUserId) : IRequest<ServiceResult<ConfirmationStatusDTO>> { }

    public class GetConfirmationStatusRequestHandler : IRequestHandler<GetConfirmationStatusRequest, ServiceResult<ConfirmationStatusDTO>>
    {
        private readonly IBookingService _bookingService;

        public GetConfirmationStatusRequestHandler(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public async Task<ServiceResult<ConfirmationStatusDTO>> Handle(GetConfirmationStatusRequest request, CancellationToken cancellationToken)
        {
            // Get booking with related entities
            var bookingResult = await _bookingService.GetByIdWithIncludeAsync(
                request.BookingId,
                b => b.Car.CarOwnerProfile,
                b => b.WorkShopProfile
            );

            if (!bookingResult.Success || bookingResult.Data == null)
            {
                return ServiceResult<ConfirmationStatusDTO>.Fail("Booking not found.");
            }

            var booking = bookingResult.Data;

            // Check if user is authorized to view this booking
            var carOwnerUserId = booking.Car.CarOwnerProfile.ApplicationUserId;
            var workshopOwnerUserId = booking.WorkShopProfile.ApplicationUserId;

            if (request.ApplicationUserId != carOwnerUserId && request.ApplicationUserId != workshopOwnerUserId)
            {
                return ServiceResult<ConfirmationStatusDTO>.Fail("You are not authorized to view this booking's confirmation status.");
            }

            // Calculate remaining seconds
            int? remainingSeconds = null;
            if (booking.ConfirmationDeadline.HasValue)
            {
                var remaining = booking.ConfirmationDeadline.Value - DateTime.UtcNow;
                remainingSeconds = remaining.TotalSeconds > 0 ? (int)remaining.TotalSeconds : 0;
            }

            var confirmationStatus = new ConfirmationStatusDTO
            {
                BookingId = booking.Id,
                CarOwnerConfirmed = booking.CarOwnerConfirmed,
                WorkshopConfirmed = booking.WorkshopOwnerConfirmed,
                Status = booking.Status.ToString(),
                ConfirmationSentAt = booking.ConfirmationSentAt,
                ConfirmationDeadline = booking.ConfirmationDeadline,
                RemainingSeconds = remainingSeconds
            };

            return ServiceResult<ConfirmationStatusDTO>.Ok(confirmationStatus, "Confirmation status retrieved successfully.");
        }
    }
}
