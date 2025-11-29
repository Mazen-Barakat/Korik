using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class GetAllRatingsByWorkShopProfileId : AbstractValidator<GetAllRatingsByWorkShopProfileIdDTO>
    {
        private readonly IWorkShopProfileService _workShopProfileService;

        public GetAllRatingsByWorkShopProfileId(IWorkShopProfileService workShopProfileService)
        {
            _workShopProfileService = workShopProfileService;

            RuleFor(x => x.WorkShopProfileId)
                .NotEmpty()
                .WithMessage("WorkShopProfileId is required.")
                .MustAsync(async (id, cancellation) => (await _workShopProfileService.IsExistAsync(id)).Data)
                .WithMessage("WorkShopProfileId does not exist.");
        }
    }
}
