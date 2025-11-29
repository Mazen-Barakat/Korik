using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class GetBookingsByWorkshopProfileIdDTOValidator
    : AbstractValidator<GetBookingsByWorkshopProfileIdDTO>
    {
        private readonly IWorkShopProfileService _workShopProfileService;

        public GetBookingsByWorkshopProfileIdDTOValidator(
            IWorkShopProfileService workShopProfileService)
        {
            _workShopProfileService = workShopProfileService;

            RuleFor(x => x.WorkshopProfileId)
                .GreaterThan(0)
                .WithMessage("Workshop Profile Id must be greater than 0.")

                .MustAsync(async (id, cancellationToken) =>
                {
                    var result = await _workShopProfileService.IsExistAsync(id);
                    return result.Data;       // true if exists
                })
                .WithMessage("Workshop Profile does not exist.");
        }
    }
}
