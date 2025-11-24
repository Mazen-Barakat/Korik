using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class CreateCategoryDTOValidator : AbstractValidator<CreateCategoryDTO>
    {
        private readonly ICategoryService _categoryService;
        public CreateCategoryDTOValidator(ICategoryService categoryService)
        {
            _categoryService = categoryService;

            // Name
            RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters long.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.")
            .MustAsync(async (name, cancellationToken) =>
            {
                var nameExistsResult = await _categoryService.HasUniqueNameAsync(name , 0);

                // service returns TRUE if name exists → invalid
                return !nameExistsResult.Data;
            })
            .WithMessage("Category name must be unique.");

            // IconURL
            RuleFor(x => x.IconURL)
                .NotEmpty().WithMessage("Icon URL is required.")
                .MaximumLength(200).WithMessage("Icon URL cannot exceed 200 characters.");


            // DisplayOrder
            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("DisplayOrder must be zero or a positive number.")
                .LessThanOrEqualTo(1000).WithMessage("DisplayOrder must be 1000 or less.");
            _categoryService = categoryService;
        }
    }
}
