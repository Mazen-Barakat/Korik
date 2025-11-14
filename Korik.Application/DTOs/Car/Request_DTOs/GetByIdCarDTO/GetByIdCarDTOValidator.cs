using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class GetByIdCarDTOValidator : AbstractValidator<GetByIdCarDTO>
    {
        public GetByIdCarDTOValidator()
        {
            // Validate Id
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than 0.");
        }
    }
}
