using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class UpdateServiceDTOValidator : AbstractValidator<UpdateServiceDTO>
    {
        private readonly IServiceService _serviceService;
        private readonly ISubcategoryService _subcategoryService;

        public UpdateServiceDTOValidator(IServiceService serviceService, ISubcategoryService subcategoryService)
        {
            _serviceService = serviceService;
            _subcategoryService = subcategoryService;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Service ID is required.")
                .MustAsync(async (id, cancellationToken) =>
                {
                    var exists = await _serviceService.IsExistAsync(id);
                    return exists.Data;
                })
                .WithMessage("Service does not exist.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Service name is required.")
                .MinimumLength(3).WithMessage("Service name must be at least 3 characters.")
                .MaximumLength(150).WithMessage("Service name cannot exceed 150 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MinimumLength(10).WithMessage("Description must be at least 10 characters.")
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

            RuleFor(x => x.Duration)
                .GreaterThan(0).WithMessage("Duration must be greater than 0 minutes.");

            RuleFor(x => x.MinPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Minimum price cannot be negative.");

            RuleFor(x => x.MaxPrice)
                .GreaterThan(0).WithMessage("Maximum price must be greater than 0.")
                .GreaterThanOrEqualTo(x => x.MinPrice)
                .WithMessage("Maximum price must be greater than or equal to minimum price.");

            RuleFor(x => x.ImageUrl)
                .NotEmpty().WithMessage("Image URL is required.")
                .MaximumLength(300).WithMessage("Image URL cannot exceed 300 characters.");

            RuleFor(x => x.SubcategoryId)
                .GreaterThan(0).WithMessage("Subcategory ID is required.")
                .MustAsync(async (subcategoryId, cancellationToken) =>
                {
                    var exists = await _subcategoryService.IsExistAsync(subcategoryId);
                    return exists.Data;
                })
                .WithMessage("Subcategory does not exist.");
        }
    }
}