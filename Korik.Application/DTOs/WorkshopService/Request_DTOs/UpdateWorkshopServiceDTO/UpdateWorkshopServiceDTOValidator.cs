using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class UpdateWorkshopServiceDTOValidator : AbstractValidator<UpdateWorkshopServiceDTO>
    {
        private readonly IWorkshopServiceService _workshopServiceService;

        public UpdateWorkshopServiceDTOValidator(IWorkshopServiceService workshopServiceService)
        {
            _workshopServiceService = workshopServiceService;

            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");

            // Duration
            RuleFor(x => x.Duration)
                .GreaterThan(0)
                .When(x => x.Duration.HasValue)
                .WithMessage("Duration must be greater than 0.");

            // Prices

            RuleFor(x => x.MinPrice)
                .GreaterThan(0)
                .When(x => x.MinPrice.HasValue)
                .WithMessage("MinPrice must be greater than 0.")
                .PrecisionScale(10, 2, true)
                .WithMessage("MaxPrice must use precision 10 and scale 2.");

            RuleFor(x => x.MaxPrice)
                .GreaterThanOrEqualTo(x => x.MinPrice)
                .When(x => x.MaxPrice.HasValue && x.MinPrice.HasValue)
                .WithMessage("MaxPrice must be greater than or equal to MinPrice.")
                .PrecisionScale(10, 2, true)
                .WithMessage("MaxPrice must use precision 10 and scale 2.");

            // Origin
            RuleFor(x => x.Origin)
                .IsInEnum()
                .When(x => x.Origin.HasValue)
                .WithMessage("Origin must be a valid CarOrigin value.");

            RuleFor(x => x)
                .MustAsync(async (dto, cancellationToken) =>
                {
                    var result = await _workshopServiceService.IsOriginUniqueForUpdateAsync(dto.Id, dto.Origin);
                    return result.Success && result.Data;
                })
                .When(x => x.Origin.HasValue)
                .WithMessage("A workshop service with this Service, Workshop, and Origin combination already exists.");
        }
    }
}