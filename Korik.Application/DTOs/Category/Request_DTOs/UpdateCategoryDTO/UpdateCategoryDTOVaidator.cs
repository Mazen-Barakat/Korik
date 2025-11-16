using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class UpdateCategoryDTOValidator : AbstractValidator<UpdateCategoryDTO>
    {
        private readonly ICategoryService _categoryService;

        public UpdateCategoryDTOValidator(ICategoryService categoryService)
        {
            _categoryService = categoryService;

            // ID must exist
            RuleFor(x => x.Id)
                .MustAsync(async (id, cancellationToken) =>
                {
                    // Check if category with this ID exists
                    var result = await _categoryService.IsExistAsync(id);
                    return result.Data;
                })
                .WithMessage("Category with the specified ID does not exist.");


            // Name
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(3).WithMessage("Name must be at least 3 characters long.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.")
                .MustAsync(async (dto, name, cancellationToken) =>
                {
                    // Check uniqueness excluding the current category
                    var nameExistsResult = await _categoryService.HasUniqueNameAsync(name, dto.Id);
                    return !nameExistsResult.Data;
                })
                .WithMessage("Category name must be unique.");

            // IconURL (any string, with max length)
            RuleFor(x => x.IconURL)
                .NotEmpty().WithMessage("Icon URL is required.")
                .MaximumLength(200).WithMessage("Icon URL cannot exceed 200 characters.");

            // DisplayOrder
            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("DisplayOrder must be zero or greater.");
        }
    }
}
