using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class PagedRequestDTOValidator : AbstractValidator<PagedRequestDTO>
    {
        public PagedRequestDTOValidator()
        {
            // Optional
            RuleFor(x => x.Name)
                .MaximumLength(150)
                .WithMessage("Workshop name cannot exceed 150 characters.");

            // Latitude & Longitude - Both must be provided together or not at all
            When(x => x.Latitude.HasValue || x.Longitude.HasValue, () =>
            {
                RuleFor(x => x.Latitude)
                    .NotNull()
                    .WithMessage("Latitude is required when Longitude is provided.")
                    .InclusiveBetween(-90m, 90m)
                    .WithMessage("Latitude must be between -90 and 90.");

                RuleFor(x => x.Longitude)
                    .NotNull()
                    .WithMessage("Longitude is required when Latitude is provided.")
                    .InclusiveBetween(-180m, 180m)
                    .WithMessage("Longitude must be between -180 and 180.");
            });

            // Optional
            RuleFor(x => x.Country)
                .MaximumLength(100)
                .When(x => !string.IsNullOrEmpty(x.Country))
                .WithMessage("Country name cannot exceed 100 characters.");

            // Optional
            RuleFor(x => x.Governorate)
                .MaximumLength(100)
                .When(x => !string.IsNullOrEmpty(x.Governorate))
                .WithMessage("Governorate name cannot exceed 100 characters.");

            // Optional
            RuleFor(x => x.City)
                .MaximumLength(100)
                .When(x => !string.IsNullOrEmpty(x.City))
                .WithMessage("City name cannot exceed 100 characters.");

            // Origin - Optional but must be valid enum if provided
            RuleFor(x => x.Origin)
                .IsInEnum()
                .When(x => x.Origin.HasValue)
                .WithMessage("Origin must be a valid CarOrigin value.");

            // WorkShopType - Optional but must be valid enum if provided
            RuleFor(x => x.WorkShopType)
                .IsInEnum()
                .When(x => x.WorkShopType.HasValue)
                .WithMessage("WorkShopType must be a valid WorkShopType value.");

            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page number must be greater than or equal to 1.");

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page size must be greater than or equal to 1.");
        }
    }
}