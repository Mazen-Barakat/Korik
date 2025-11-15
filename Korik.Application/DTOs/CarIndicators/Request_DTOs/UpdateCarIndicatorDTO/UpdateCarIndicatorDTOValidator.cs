using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Korik.Domain;

namespace Korik.Application
{
    public class UpdateCarIndicatorDTOValidator : AbstractValidator<UpdateCarIndicatorDTO>
    {
        private readonly ICarIndicatorService _carIndicatorService;
        private readonly ICarService _carService;

        public UpdateCarIndicatorDTOValidator(
            ICarIndicatorService carIndicatorService,
            ICarService carService)
        {
            _carIndicatorService = carIndicatorService;
            _carService = carService;

            // Validate Id
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than 0.")
                .MustAsync(async (id, cancellation) =>
                {
                    var result = await _carIndicatorService.IsExistAsync(id);
                    return result.Success && result.Data;
                }).WithMessage("No car indicator exists with the provided Id.");

            // Validate IndicatorType
            RuleFor(x => x.IndicatorType)
                .IsInEnum().WithMessage("Invalid IndicatorType.");

            // Validate CarStatus
            RuleFor(x => x.CarStatus)
                .IsInEnum().WithMessage("Invalid CarStatus.");

            // Validate LastCheckedDate
            RuleFor(x => x.LastCheckedDate)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("LastCheckedDate cannot be in the future.");

            // Validate NextCheckedDate
            RuleFor(x => x.NextCheckedDate)
                .GreaterThan(x => x.LastCheckedDate).WithMessage("NextCheckedDate must be after LastCheckedDate.");

            // Validate CarId
            RuleFor(x => x.CarId)
                .GreaterThan(0).WithMessage("CarId must be greater than 0.")
                .MustAsync(async (carId, cancellation) =>
                {
                    var result = await _carService.IsExistAsync(carId);
                    return result.Success && result.Data;
                }).WithMessage("No car exists with the provided CarId.");
        }
    }
}
