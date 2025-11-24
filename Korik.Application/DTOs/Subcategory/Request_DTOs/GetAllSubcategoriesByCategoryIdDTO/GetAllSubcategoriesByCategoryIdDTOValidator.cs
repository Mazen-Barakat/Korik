using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class GetAllSubcategoriesByCategoryIdDTOValidator : AbstractValidator<GetAllSubcategoriesByCategoryIdDTO>
    {
        private readonly ICategoryService _categoryService;
        public GetAllSubcategoriesByCategoryIdDTOValidator(ICategoryService categoryService)
        {
            _categoryService = categoryService;

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("CategoryId must be greater than zero.")
                .MustAsync(async (categoryId, cancellation) =>
                {
                    var result = await _categoryService.IsExistAsync(categoryId);
                    return result.Data;   // true if exists, false if not
                })
                .WithMessage("Category does not exist.");
        }
    }

}
