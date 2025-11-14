using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class DeleteCarExpenseDTOValidator : AbstractValidator<DeleteCarExpenseDTO>
    {
        public DeleteCarExpenseDTOValidator()
        {
            // Validate Id
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than 0.");
        }
    }
}
