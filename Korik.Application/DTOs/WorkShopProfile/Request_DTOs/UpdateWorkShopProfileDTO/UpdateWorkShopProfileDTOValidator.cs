using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class UpdateWorkShopProfileDTOValidator : AbstractValidator<UpdateWorkShopProfileDTO>
    {
        private readonly IWorkShopProfileService _workShopProfileService;
        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public UpdateWorkShopProfileDTOValidator(IWorkShopProfileService workShopProfileService)
        {
            _workShopProfileService = workShopProfileService;

            // Name
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Workshop name is required.")
                .MaximumLength(150).WithMessage("Workshop name cannot exceed 150 characters.");

            // Description
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(1000).WithMessage("Description cannot exceed 500 characters.");

            // Phone Number
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters.")
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

            // Numbers of technicians
            RuleFor(x => x.NumbersOfTechnicians)
                .GreaterThanOrEqualTo(1).WithMessage("Number of technicians must be at least 1.")
                .LessThanOrEqualTo(500).WithMessage("Number of technicians cannot exceed 500.");

            // Latitude
            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessage("Latitude must be between -90 and 90.");

            // Longitude
            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180)
                .WithMessage("Longitude must be between -180 and 180.");

            // Workshop Type
            RuleFor(x => x.WorkShopType)
                .IsInEnum()
                .WithMessage("Invalid workshop type.");

            // Licence Image URL (optional)
            RuleFor(x => x.LicenceImageUrl)
                .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.LicenceImageUrl))
                .WithMessage("License image URL must be a valid URL.");

            // Logo Image URL (optional)
            RuleFor(x => x.LogoImageUrl)
                .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.LogoImageUrl))
                .WithMessage("Logo image URL must be a valid URL.");

            // Licence Image File
            RuleFor(x => x.LicenceImage)
                .Must(BeAValidFileSize).When(x => x.LicenceImage != null)
                .WithMessage($"License image must be at most {_maxFileSize / 1024 / 1024}MB.")
                .Must(BeAValidFileType).When(x => x.LicenceImage != null)
                .WithMessage($"License image type must be one of: {string.Join(", ", _allowedExtensions)}");

            // Logo Image File
            RuleFor(x => x.LogoImage)
                .Must(BeAValidFileSize).When(x => x.LogoImage != null)
                .WithMessage($"Logo image must be at most {_maxFileSize / 1024 / 1024}MB.")
                .Must(BeAValidFileType).When(x => x.LogoImage != null)
                .WithMessage($"Logo image type must be one of: {string.Join(", ", _allowedExtensions)}");

            // Authorization Check
            RuleFor(x => x)
                .MustAsync(async (dto, cancellation) =>
                    await UserOwnsWorkshop(dto.Id, dto.ApplicationUserId, dto))
                .WithMessage("You are not authorized to update this workshop profile.");
        }

        // ----------------- Helper Methods -----------------

        private bool BeAValidUrl(string? url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
                   (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        private bool BeAValidFileSize(IFormFile? file)
        {
            if (file == null) return true;
            return file.Length > 0 && file.Length <= _maxFileSize;
        }

        private bool BeAValidFileType(IFormFile? file)
        {
            if (file == null) return true;
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return _allowedExtensions.Contains(extension);
        }

        private async Task<bool> UserOwnsWorkshop(int profileId, string? applicationUserId, UpdateWorkShopProfileDTO dto)
        {
            if (string.IsNullOrEmpty(applicationUserId))
                return false;

            var workshop = await _workShopProfileService.GetByIdAsync(profileId);

            if (workshop == null)
                return false;

            return workshop.Data?.ApplicationUserId == applicationUserId;
        }
    }
}