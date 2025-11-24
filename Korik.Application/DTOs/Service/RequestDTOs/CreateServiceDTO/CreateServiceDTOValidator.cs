using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class CreateServiceDTOValidator : AbstractValidator<CreateServiceDTO>
    {
        public CreateServiceDTOValidator(ISubcategoryService subcategoryService)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Service name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Duration)
                .GreaterThan(0).WithMessage("Duration must be greater than zero.");

            RuleFor(x => x.MinPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Min price cannot be negative.");

            RuleFor(x => x.MaxPrice)
                .GreaterThan(0).WithMessage("Max price must be greater than zero.");

            RuleFor(x => x)
                .Must(x => x.MinPrice <= x.MaxPrice)
                .WithMessage("MinPrice cannot be greater than MaxPrice.");
           
            RuleFor(x => x.ImageUrl)
                .MustAsync(async (image, ct) =>
                {
                    if (string.IsNullOrWhiteSpace(image))
                        return true; // Optional field

                    if (image.Length > 100)
                        return false; // Limit characters

                    // Allowed extensions
                    var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

                    // Only allow valid extensions
                    bool isValidExtension = validExtensions.Any(ext =>
                        image.EndsWith(ext, StringComparison.OrdinalIgnoreCase));

                    // Reject unsafe characters
                    bool hasInvalidChars = image.Contains("..") || image.Contains("~") || image.Contains("\\") || image.Contains("/");

                    return isValidExtension && !hasInvalidChars;
                })
                .WithMessage("Image must be a valid file name ending with .jpg, .jpeg, .png, or .webp and max 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl));


            RuleFor(x => x.SubcategoryId)
                .GreaterThan(0).WithMessage("SubcategoryId is required.")
                .MustAsync(async (subcategoryId, cancellationToken) =>
                {
                    var result = await subcategoryService.IsExistAsync(subcategoryId);
                    return result.Data; 
                })
                .WithMessage("The selected Subcategory does not exist.");
        }
    }

}
