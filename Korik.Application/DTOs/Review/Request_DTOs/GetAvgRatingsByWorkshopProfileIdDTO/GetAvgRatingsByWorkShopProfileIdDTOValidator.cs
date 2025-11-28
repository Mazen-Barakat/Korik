using FluentValidation;
using Korik.Application;

namespace Korik.Application
{
    public class GetAvgRatingsByWorkShopProfileIdDTOValidator : AbstractValidator<GetAvgRatingsByWorkShopProfileIdDTO>
    {
        private readonly IWorkShopProfileService _workShopProfileService;

        public GetAvgRatingsByWorkShopProfileIdDTOValidator(IWorkShopProfileService workShopProfileService)
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
