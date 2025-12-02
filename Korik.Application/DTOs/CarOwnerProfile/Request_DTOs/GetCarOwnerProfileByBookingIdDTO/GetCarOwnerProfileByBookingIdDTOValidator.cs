using FluentValidation;

namespace Korik.Application 
{
    public class GetCarOwnerProfileByBookingIdDTOValidator : AbstractValidator<GetCarOwnerProfileByBookingIdDTO>
    {
        private readonly IBookingService _bookingService;

        public GetCarOwnerProfileByBookingIdDTOValidator(IBookingService bookingService)
        {
            _bookingService = bookingService;

            RuleFor(x => x.BookingId)
                .GreaterThan(0)
                .WithMessage("Booking ID must be greater than 0.")

                .MustAsync(async (bookingId, cancellationToken) =>
                {
                    var result = await _bookingService.IsExistAsync(bookingId);
                    return result.Data;
                })
                .WithMessage("Booking does not exist.");
        }
    }
}
