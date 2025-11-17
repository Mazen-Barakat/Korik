using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class DeleteWorkShopPhotosByIdDTOValidator : AbstractValidator<DeleteWorkShopPhotosByIdDTO>
    {
        private readonly IWorkShopPhotoService _workShopPhotoService;

        public DeleteWorkShopPhotosByIdDTOValidator(IWorkShopPhotoService workShopPhotoService)
        {
            _workShopPhotoService = workShopPhotoService;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than 0.")
                .MustAsync(ExistInDatabase).WithMessage("The workshop Image does not exist.");
        }

        private async Task<bool> ExistInDatabase(int id, CancellationToken cancellationToken)
        {
            var result = await _workShopPhotoService.IsExistAsync(id);
            return result.Success && result.Data;
        }
    }
}