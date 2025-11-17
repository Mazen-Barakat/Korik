using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{

    public class DeleteSubcategoryDTOValidator : AbstractValidator<DeleteSubcategoryDTO>
    {
        public DeleteSubcategoryDTOValidator(ISubcategoryService subcategoryService)
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than zero.")
                .MustAsync(async (id, cancellation) =>
                {
                    var result = await subcategoryService.IsExistAsync(id);

                    // result.Data contains the boolean result
                    return result.Data;
                })
                .WithMessage("Subcategory does not exist.");
        }
    }
}
