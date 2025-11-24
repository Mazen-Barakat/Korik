using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class GetAllWorkShopProfilesByServiceDTOValidator : AbstractValidator<GetAllWorkShopProfilesByServiceDTO>
    {
        public GetAllWorkShopProfilesByServiceDTOValidator()
        {
            RuleFor(x => x.ServiceId)
                .GreaterThan(0).WithMessage("Id must be greater than 0.");

            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page number must be greater than or equal to 1.");

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page size must be greater than or equal to 1.");
        }
    }
}