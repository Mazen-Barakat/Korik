using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class GetWorkShopProfileByIdDTOValidator : AbstractValidator<GetWorkShopProfileByIdDTO>
    {
        private readonly IWorkShopProfileService _workShopProfileService;

        public GetWorkShopProfileByIdDTOValidator(IWorkShopProfileService workShopProfileService)
        {
            _workShopProfileService = workShopProfileService;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than 0.")
                .MustAsync(ExistInDatabase).WithMessage("The workshop profile with the specified Id does not exist.");
        }

        private async Task<bool> ExistInDatabase(int id, CancellationToken cancellationToken)
        {
            var result = await _workShopProfileService.IsExistAsync(id);
            return result.Success && result.Data;
        }
    }
}