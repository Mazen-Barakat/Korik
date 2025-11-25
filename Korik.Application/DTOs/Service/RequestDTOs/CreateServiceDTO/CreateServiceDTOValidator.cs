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
