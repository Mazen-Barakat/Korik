using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class CreateWorkshopServiceDTOValidator : AbstractValidator<CreateWorkshopServiceDTO>
    {
        private readonly IServiceService _serviceService;
        private readonly IWorkshopServiceService _workshopServiceService;

        public CreateWorkshopServiceDTOValidator(IServiceService serviceService, IWorkshopServiceService workshopServiceService)
        {
            _serviceService = serviceService;
            _workshopServiceService = workshopServiceService;

            // Foreign Keys

            RuleFor(x => x.ServiceId)
                .GreaterThan(0)
                .WithMessage("ServiceId must be greater than 0.")
                .MustAsync(async (id, cancellationToken) =>
                {
                    var exists = await _serviceService.IsExistAsync(id);
                    return exists.Data;
                })
                .WithMessage("Service does not exist.");

            RuleFor(x => x.WorkShopProfileId)
                .GreaterThan(0)
                .WithMessage("WorkShopProfileId must be greater than 0.");

            // Replace the problematic block with the following:

            RuleFor(x => new { x.ServiceId, x.WorkShopProfileId, x.Origin })
                .MustAsync(async (dto, cancellationToken) =>
                {
                    var result = await _workshopServiceService.GetAllAsync();
                    var exists = result.Data.Any(ws =>
                        ws.ServiceId == dto.ServiceId &&
                        ws.WorkShopProfileId == dto.WorkShopProfileId &&
                        ws.Origin == dto.Origin);
                    return !exists; // Ensure the combination does not exist
                })
                .WithMessage("The Service for this WorkShop with this origin already exists!");
            // Duration
            RuleFor(x => x.Duration)
                .GreaterThan(0)
                .WithMessage("Duration must be greater than 0.");

            // Prices

            RuleFor(x => x.MinPrice)
                .GreaterThan(0)
                .WithMessage("MinPrice must be greater than 0.")
                .PrecisionScale(10, 2, true)
                .WithMessage("MaxPrice must use precision 10 and scale 2.");

            RuleFor(x => x.MinPrice)
                .GreaterThanOrEqualTo(x => x.MinPrice)
                .WithMessage("MaxPrice must be greater than or equal to MinPrice.")
                .PrecisionScale(10, 2, true)
                .WithMessage("MaxPrice must use precision 10 and scale 2.");

            // Origin
            RuleFor(x => x.Origin)
                .IsInEnum()
                .WithMessage("Origin must be a valid CarOrigin value.");
        }
    }
}