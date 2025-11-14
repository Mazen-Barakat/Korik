using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class GetCarsByCarOwnerProfileIdDTOValidator : AbstractValidator<GetCarsByCarOwnerProfileIdDTO>
    {
        public GetCarsByCarOwnerProfileIdDTOValidator()
        {
            RuleFor(x => x.CarOwnerProfileId)
                .GreaterThan(0).WithMessage("CarOwnerProfileId must be a positive integer.")
                .NotEmpty().WithMessage("CarOwnerProfileId is required.");
        }
    }
}
