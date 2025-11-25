using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{

    public class CreateSubcategoryDTOValidator : AbstractValidator<CreateSubcategoryDTO>
    {
        private readonly ICategoryService _categoryService;

        public CreateSubcategoryDTOValidator(ICategoryService categoryService)
        {
            _categoryService = categoryService;

            // Name
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(3).WithMessage("Name must be at least 3 characters long.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            // CategoryId (using MustAsync inline)
            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Category Id must be greater than zero.")
                .MustAsync(async (categoryId, cancellationToken) =>
                {
                    var existsResult = await _categoryService.IsExistAsync(categoryId);

                    // existsResult.Data → true/false
                    // existsResult.Success → service call succeeded
                    return existsResult.Success && existsResult.Data;
                })
                .WithMessage("Category does not exist.");
        }
    }

}
