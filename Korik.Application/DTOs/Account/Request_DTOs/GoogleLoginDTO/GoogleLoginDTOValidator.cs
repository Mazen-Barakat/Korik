using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class GoogleLoginDTOValidator : AbstractValidator<GoogleLoginDTO>
    {
        public GoogleLoginDTOValidator()
        {
            RuleFor(x => x.IdToken)
                .NotEmpty().WithMessage("IdToken is required.");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required.")
                .Must(role => role == "CAROWNER" || role == "WORKSHOP")
                .WithMessage("Role must be either 'CAROWNER' or 'WORKSHOP'.");

        }
    }
}
