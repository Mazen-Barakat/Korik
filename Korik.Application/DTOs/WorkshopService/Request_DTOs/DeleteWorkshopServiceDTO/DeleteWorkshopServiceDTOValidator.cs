using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class DeleteWorkshopServiceDTOValidator : AbstractValidator<DeleteWorkshopServiceDTO>
    {
        private readonly IWorkshopServiceService _workshopServiceService;

        public DeleteWorkshopServiceDTOValidator(IWorkshopServiceService workshopServiceService)
        {
            _workshopServiceService = workshopServiceService;

            // Validate ID exists in database
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.")
                .MustAsync(async (id, cancellationToken) =>
                {
                    var result = await _workshopServiceService.IsExistAsync(id);
                    return result.Data;
                })
                .WithMessage("Workshop service with the specified ID does not exist.");
        }
    }
}