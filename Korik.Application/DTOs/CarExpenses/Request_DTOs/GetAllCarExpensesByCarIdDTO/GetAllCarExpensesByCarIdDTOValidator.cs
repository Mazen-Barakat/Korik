using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Korik.Application
{
    public class GetAllCarExpensesByCarIdDTOValidator : AbstractValidator<GetAllCarExpensesByCarIdDTO>
    {
        public GetAllCarExpensesByCarIdDTOValidator()
        {
            // Validate CarId
            RuleFor(x => x.CarId)
                .NotEmpty().WithMessage("CarId is required.");
        }
    }
}
