using System;
using FluentValidation;
using Korik.Domain;

namespace Korik.Application
{
    public class CreateCarIndicatorDTOValidator : AbstractValidator<CreateCarIndicatorDTO>
    {
        private readonly ICarService _carService;

        public CreateCarIndicatorDTOValidator(ICarService carService)
        {
            _carService = carService;

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
