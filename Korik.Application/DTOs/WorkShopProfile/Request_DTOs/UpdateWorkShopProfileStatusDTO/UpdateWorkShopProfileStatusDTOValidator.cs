using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class UpdateWorkShopProfileStatusDTOValidator : AbstractValidator<UpdateWorkShopProfileStatusDTO>
    {
        public UpdateWorkShopProfileStatusDTOValidator()
        {
            RuleFor(x => x.VerificationStatus)
                .IsInEnum()
                .WithMessage("Invalid Verification Status type.");
        }
    }
}