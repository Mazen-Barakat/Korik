using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class UpdateSubcategoryDTOValidator : AbstractValidator<UpdateSubcategoryDTO>
    {
        private readonly ISubcategoryService _subcategoryService;
        private readonly ICategoryService _categoryService;


        public UpdateSubcategoryDTOValidator(
            ISubcategoryService subcategoryService,
            ICategoryService categoryService)
        {
            _subcategoryService = subcategoryService;
            _categoryService = categoryService;

            // ----- Validate Subcategory Id -----
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Subcategory Id must be greater than zero.")
                .MustAsync(async (id, cancellationToken) =>
                {
                    var result = await _subcategoryService.IsExistAsync(id);
                    return result.Success && result.Data;
                })
                .WithMessage("Subcategory with the specified Id does not exist.");

            // ----- Name -----
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(3).WithMessage("Name must be at least 3 characters long.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            // ----- Description -----
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            // ----- Validate Category Id (using ICategoryService) -----
            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Category Id must be greater than zero.")
                .MustAsync(async (categoryId, cancellationToken) =>
                {
                    var result = await _categoryService.IsExistAsync(categoryId);
                    return result.Success && result.Data;
                })
                .WithMessage("Category with the specified Id does not exist.");
        }
    }
}
