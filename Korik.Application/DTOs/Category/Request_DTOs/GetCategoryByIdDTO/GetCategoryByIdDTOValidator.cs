using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class GetCategoryByIdDTOValidator  : AbstractValidator<GetCategoryByIdDTO>
    {
        private readonly ICategoryService _categoryService;
        public GetCategoryByIdDTOValidator(ICategoryService categoryService)
        {
            _categoryService = categoryService;

            RuleFor(x => x.Id)
                .MustAsync(async (id, cancellationToken) =>
                {
                    // Check if category with this ID exists
                    var result = await _categoryService.IsExistAsync(id);

                    return result.Data;
                })
                .WithMessage("Category with the specified ID does not exist.");
            _categoryService = categoryService;
        }

    }
}
