using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class GetSubcategoryByIdDTOValidator : AbstractValidator<GetSubcategoryByIdDTO>
    {
        public GetSubcategoryByIdDTOValidator(ISubcategoryService subcategoryService)
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than zero.")
                .MustAsync(async (id, cancellation) =>
                {
                    var result = await subcategoryService.IsExistAsync(id);
                    return result.Data; // true = exists
                })
                .WithMessage("Subcategory not found.");
        }
    }

}
