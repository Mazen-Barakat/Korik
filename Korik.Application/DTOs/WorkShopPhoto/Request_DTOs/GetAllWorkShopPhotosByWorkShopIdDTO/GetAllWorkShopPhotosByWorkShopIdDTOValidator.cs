using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class GetAllWorkShopPhotosByWorkShopIdDTOValidator : AbstractValidator<GetAllWorkShopPhotosByWorkShopIdDTO>
    {
        private readonly IWorkShopProfileService _workShopProfileService;

        public GetAllWorkShopPhotosByWorkShopIdDTOValidator(IWorkShopProfileService workShopProfileService)
        {
            _workShopProfileService = workShopProfileService;

            RuleFor(x => x.WorkShopProfileId)
                    .GreaterThan(0).WithMessage("Id must be greater than 0.")
                    .MustAsync(ExistInDatabase).WithMessage("The workshop does not exist.");
        }

        private async Task<bool> ExistInDatabase(int WorkShopProfileId, CancellationToken cancellationToken)
        {
            var result = await _workShopProfileService.IsExistAsync(WorkShopProfileId);
            return result.Success && result.Data;
        }
    }
}