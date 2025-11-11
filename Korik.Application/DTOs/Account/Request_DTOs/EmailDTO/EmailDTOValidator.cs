using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class EmailDTOValidator : AbstractValidator<EmailDTO>
    {
        public EmailDTOValidator()
        {
            RuleFor(x => x.To)
           .NotEmpty().WithMessage("Recipient email is required.")
           .EmailAddress().WithMessage("Invalid email address format.");

            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("Subject is required.")
                .MaximumLength(100).WithMessage("Subject cannot exceed 100 characters.");

            RuleFor(x => x.Body)
                .NotEmpty().WithMessage("Email body is required.")
                .MinimumLength(10).WithMessage("Email body must be at least 10 characters long.");
        }
    }

}
