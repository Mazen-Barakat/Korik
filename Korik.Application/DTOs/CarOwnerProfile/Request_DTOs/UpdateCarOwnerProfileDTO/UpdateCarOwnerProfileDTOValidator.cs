using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class UpdateCarOwnerProfileDTOValidator : AbstractValidator<UpdateCarOwnerProfileDTO>
    {
        private readonly ICarOwnerProfileService _carOwnerProfileService;
        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public UpdateCarOwnerProfileDTOValidator(ICarOwnerProfileService carOwnerProfileService)
        {
            _carOwnerProfileService = carOwnerProfileService;

            // First Name
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(100).WithMessage("First name cannot exceed 100 characters.");

            // Last Name
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters.");

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

            // Profile Image URL (optional)
            RuleFor(x => x.ProfileImageUrl)
                .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.ProfileImageUrl))
                .WithMessage("Profile image URL must be a valid URL.");

            // Profile Image File Validation (NEW)
            RuleFor(x => x.ProfileImage)
                .Must(BeAValidFileSize).When(x => x.ProfileImage != null)
                .WithMessage($"Image file size must not exceed {_maxFileSize / 1024 / 1024}MB.")
                .Must(BeAValidFileType).When(x => x.ProfileImage != null)
                .WithMessage($"Image file type must be one of: {string.Join(", ", _allowedExtensions)}");

            // Preferred Language
            RuleFor(x => x.PreferredLanguage)
                .IsInEnum()
                .WithMessage("Preferred language must be either English or Arabic.");

            // Authorization Check - User must own this profile
            RuleFor(x => x)
                .MustAsync(async (dto, cancellationToken) =>
                    await UserOwnsProfile(dto.Id, dto.ApplicationUserId, cancellationToken))
                .WithMessage("You are not authorized to update this profile.");

            // Profile ID - Check if profile exists and user owns it
        }

        private bool BeAValidUrl(string? url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        private bool BeAValidFileSize(IFormFile? file)
        {
            if (file == null) return true; // Null is valid (optional)
            return file.Length > 0 && file.Length <= _maxFileSize;
        }

        private bool BeAValidFileType(IFormFile? file)
        {
            if (file == null) return true; // Null is valid (optional)

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return _allowedExtensions.Contains(extension);
        }

        private async Task<bool> UserOwnsProfile(int profileId, string? applicationUserId, CancellationToken cancellationToken)
        {
            // If ApplicationUserId is not set, deny access
            if (string.IsNullOrEmpty(applicationUserId))
                return false;

            // Get the profile by ID (without tracking)
            var profile = await _carOwnerProfileService.GetByIdAsync(profileId);

            // Profile doesn't exist
            if (profile == null)
                return false;

            return profile?.Data?.ApplicationUserId == applicationUserId;
        }
    }
}