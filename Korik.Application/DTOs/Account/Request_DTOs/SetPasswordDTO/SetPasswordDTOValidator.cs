using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class SetPasswordDTOValidator : AbstractValidator<SetPasswordDTO>
    {
        public SetPasswordDTOValidator()
        {
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"\d").WithMessage("Password must contain at least one number.")
                .Matches(@"[\W_]").WithMessage("Password must contain at least one special character.")
                .Must(HaveRequiredUniqueChars).WithMessage("Password must contain at least 1 unique character.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.NewPassword)
                .WithMessage("Passwords do not match.");
        }

        private bool HaveRequiredUniqueChars(string password)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            // Equivalent to RequiredUniqueChars = 1
            return password.Distinct().Count() >= 1;
        }
    }
}
