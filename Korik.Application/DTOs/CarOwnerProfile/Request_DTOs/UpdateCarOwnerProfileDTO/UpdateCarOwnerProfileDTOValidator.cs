using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class UpdateCarOwnerProfileDTOValidator : AbstractValidator<UpdateCarOwnerProfileDTO>
    {
        public UpdateCarOwnerProfileDTOValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

            // Last Name
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

            // Phone Number
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^(?:\+201|01)[0-2,5][0-9]{8}$")
                .WithMessage("Phone number must be a valid Egyptian number (e.g., 01012345678 or +201012345678).");

            // Country
            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required.")
                .MaximumLength(100).WithMessage("Country name cannot exceed 100 characters.");

            // Governorate
            RuleFor(x => x.Governorate)
                .NotEmpty().WithMessage("Governorate is required.")
                .MaximumLength(100).WithMessage("Governorate name cannot exceed 100 characters.");

            // City
            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(100).WithMessage("City name cannot exceed 100 characters.");

            // Profile Image (optional)
            RuleFor(x => x.ProfileImageUrl)
                .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.ProfileImageUrl))
                .WithMessage("Profile image URL must be a valid URL.");

            // Preferred Language
            RuleFor(x => x.PreferredLanguage)
                .IsInEnum()
                .WithMessage("Preferred language must be either English or Arabic.");
        }

        private bool BeAValidUrl(string? url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}