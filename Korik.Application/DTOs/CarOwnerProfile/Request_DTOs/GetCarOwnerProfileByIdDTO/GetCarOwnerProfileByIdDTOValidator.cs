using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class GetCarOwnerProfileByIdDTOValidator : AbstractValidator<GetCarOwnerProfileByIdDTO>
    {
        public GetCarOwnerProfileByIdDTOValidator()
        {
            RuleFor(x => x.ApplicationUserId)
                .NotEmpty().WithMessage("ApplicationUserId is required.")
                .Must(id => Guid.TryParse(id, out _))
                .WithMessage("ApplicationUserId must be a valid GUID.");
        }
    }
}