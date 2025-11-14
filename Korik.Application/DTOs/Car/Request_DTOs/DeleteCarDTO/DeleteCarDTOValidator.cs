using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Korik.Application
{
    public class DeleteCarDTOValidator : AbstractValidator<DeleteCarDTO>
    {
        public DeleteCarDTOValidator()
        {
            // Validate Id
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than 0.");
        }
    }
}
