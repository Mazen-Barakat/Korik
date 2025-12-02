using FluentValidation;

namespace Korik.Application
{
    public class GetCarOwnerProfileByBookingIdValidator : AbstractValidator<GetCarOwnerProfileByBookingIdDTO>
    {
        public GetCarOwnerProfileByBookingIdValidator()
        {
            RuleFor(x => x.BookingId)
                .GreaterThan(0)
                .WithMessage("Booking ID must be greater than 0.");
        }
    }
}
