using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class GetWorkshopServicesByProfileIDDTOValidator : AbstractValidator<GetWorkshopServicesByProfileIDDTO>
    {
        private readonly IWorkShopProfileService _workShopProfileService;

        public GetWorkshopServicesByProfileIDDTOValidator(IWorkShopProfileService workShopProfileService)
        {
            _workShopProfileService = workShopProfileService;

            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("workshop id is required")
                .GreaterThanOrEqualTo(1)
                .WithMessage("no workshop with this Id")
                .MustAsync(async (id, cancellationToken) =>
                {
                    var exist = await _workShopProfileService.IsExistAsync(id);

                    return exist.Data;
                })
                .WithMessage("workshop is not found");

            RuleFor(x => x.PageNumber)
               .GreaterThanOrEqualTo(1)
               .WithMessage("Page number must be greater than or equal to 1.");

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page size must be greater than or equal to 1.");
        }
    }
}