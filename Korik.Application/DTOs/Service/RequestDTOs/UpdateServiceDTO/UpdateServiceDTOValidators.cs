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
        public UpdateServiceDTOValidator(
            ISubcategoryService subcategoryService,
            IServiceService serviceService)
        {
            _serviceService = serviceService;
            _subcategoryService = subcategoryService;
            // ---- ID ----
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id is required.")
                .MustAsync(async (id, cancellationToken) =>
                {
                    var result = await _serviceService.IsExistAsync(id);
                    return result.Data; // true if exists
                })
                .WithMessage("The specified Service does not exist.");

            // ---- NAME ----
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Service name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            // ---- DESCRIPTION ----
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Description));

            // ---- SUBCATEGORY ----
            RuleFor(x => x.SubcategoryId)
                .GreaterThan(0).WithMessage("SubcategoryId is required.")
                .MustAsync(async (subcategoryId, cancellationToken) =>
                {
                    var result = await _subcategoryService.IsExistAsync(subcategoryId);
                    return result.Data;
                })
                .WithMessage("The selected Subcategory does not exist.");
        }
    }


}
