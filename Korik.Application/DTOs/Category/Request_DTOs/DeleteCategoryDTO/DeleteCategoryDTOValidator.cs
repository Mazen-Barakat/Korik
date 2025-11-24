using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{ 
    public class DeleteCategoryDTOValidator : AbstractValidator<DeleteCategoryDTO>
    {
        private readonly ICategoryService _categoryService;
        public DeleteCategoryDTOValidator(ICategoryService categoryService)
        {
            _categoryService = categoryService;

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Category ID is required.")
                .MustAsync(async (id, cancellationToken) =>
                {
                    // Check if category with this ID exists
                    var result = await _categoryService.IsExistAsync(id);
                    
                    return result.Data;
                })
                .WithMessage("Category with the specified ID does not exist.");
        }
    }
}
