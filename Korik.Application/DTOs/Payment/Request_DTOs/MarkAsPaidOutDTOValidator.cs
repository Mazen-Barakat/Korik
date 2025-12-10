using FluentValidation;

namespace Korik.Application
{
    public class MarkAsPaidOutDTOValidator : AbstractValidator<MarkAsPaidOutDTO>
    {
        public MarkAsPaidOutDTOValidator()
        {
            RuleFor(x => x.PaymentId)
                .GreaterThan(0)
                .WithMessage("PaymentId must be greater than 0.");

            RuleFor(x => x.PayoutMethod)
                .NotEmpty()
                .WithMessage("PayoutMethod is required.")
                .MaximumLength(50)
                .WithMessage("PayoutMethod cannot exceed 50 characters.")
                .Must(method => new[] { "BankTransfer", "Cash", "Check", "Stripe", "Other" }.Contains(method))
                .WithMessage("PayoutMethod must be one of: BankTransfer, Cash, Check, Stripe, Other");

            RuleFor(x => x.PayoutReference)
                .MaximumLength(255)
                .WithMessage("PayoutReference cannot exceed 255 characters.");

            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .WithMessage("Notes cannot exceed 1000 characters.");
        }
    }
}
