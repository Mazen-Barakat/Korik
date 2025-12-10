using FluentValidation;

namespace Korik.Application
{
    public class CreatePaymentDTOValidator : AbstractValidator<CreatePaymentDTO>
    {
        public CreatePaymentDTOValidator()
        {
            RuleFor(x => x.BookingId)
                .GreaterThan(0)
                .WithMessage("BookingId must be greater than 0.");
        }
    }
}
