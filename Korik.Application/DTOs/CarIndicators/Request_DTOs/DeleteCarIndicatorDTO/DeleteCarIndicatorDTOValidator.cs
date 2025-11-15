using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Korik.Domain;

namespace Korik.Application
{
    public class DeleteCarIndicatorDTOValidator : AbstractValidator<DeleteCarIndicatorDTO>
    {
        private readonly ICarIndicatorService _carIndicatorService;

        public DeleteCarIndicatorDTOValidator(ICarIndicatorService carIndicatorService)
        {
            _carIndicatorService = carIndicatorService;

            // Validate Id
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than 0.")
                .MustAsync(async (id, cancellation) =>
                {
                    var result = await _carIndicatorService.IsExistAsync(id);
                    return result.Success && result.Data;
                }).WithMessage("No car indicator exists with the provided Id.");
        }
    }
}
