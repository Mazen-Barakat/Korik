using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class SearchWorkshopsByServiceAndOriginDTOValidator : AbstractValidator<SearchWorkshopsByServiceAndOriginDTO>
    {
        private readonly IServiceService _serviceService;

        public SearchWorkshopsByServiceAndOriginDTOValidator(IServiceService serviceService)
        {
            _serviceService = serviceService;

            RuleFor(x => x.ServiceId)
                 .GreaterThan(0)
                 .WithMessage("ServiceId must be greater than 0.")
                 .MustAsync(async (id, cancellationToken) =>
                 {
                     var exists = await _serviceService.IsExistAsync(id);
                     return exists.Data;
                 })
                 .WithMessage("Service does not exist.");

            RuleFor(x => x.Origin)
                .IsInEnum()
                .When(x => x.Origin.HasValue)
                .WithMessage("Origin must be a valid CarOrigin value.");

            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page number must be greater than or equal to 1.");

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page size must be greater than or equal to 1.");
            _serviceService = serviceService;
        }
    }
}