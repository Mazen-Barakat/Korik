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
        private readonly ICarService _carService;
        private readonly ICarIndicatorService _indicatorService;
        private readonly ICarIndicatorStatusService _statusService;

        public UpdateCarIndicatorDTOValidator(
            ICarService carService,
            ICarIndicatorService indicatorService,
            ICarIndicatorStatusService statusService)
        {
            _carService = carService;
            _indicatorService = indicatorService;
            _statusService = statusService;

            // -----------------------------------------------------
            // 1️⃣ Id must exist
            // -----------------------------------------------------
            RuleFor(x => x.Id)
                .MustAsync(async (id, ct) =>
                {
                    var result = await _indicatorService.IsExistAsync(id);
                    return result.Success && result.Data;
                })
                .WithMessage("The specified indicator does not exist.");

            // -----------------------------------------------------
            // 2️⃣ Car must exist
            // -----------------------------------------------------
            RuleFor(x => x.CarId)
                .MustAsync(async (carId, ct) =>
                {
                    var result = await _carService.IsExistAsync(carId);
                    return result.Success && result.Data;
                })
                .WithMessage("The specified car does not exist.");

            // -----------------------------------------------------
            // 3️⃣ Ensure indicator belongs to this car
            // -----------------------------------------------------
            RuleFor(x => x)
                .MustAsync(async (dto, ct) =>
                {
                    var indicator = await _indicatorService.GetByIdAsync(dto.Id);

                    if (indicator == null || indicator.Data == null)
                        return false;

                    return indicator.Data.CarId == dto.CarId;
                })
                .WithMessage("This indicator does not belong to the specified car.");

            // -----------------------------------------------------
            // 4️⃣ Recalculate business fields (same as Create)
            // -----------------------------------------------------
            RuleFor(x => x)
                .CustomAsync(async (dto, context, ct) =>
                {
                    var carResult = await _carService.GetByIdAsync(dto.CarId);
                    if (carResult == null || carResult.Data == null)
                    {
                        context.AddFailure("Car not found.");
                        return;
                    }

                    if (dto.LastCheckedDate == default || dto.NextCheckedDate == default)
                    {
                        context.AddFailure("LastCheckedDate and NextCheckedDate must be valid.");
                        return;
                    }

                    var calculated = _statusService.CalculateAll(
                        dto.IndicatorType,
                        dto.LastCheckedDate,
                        dto.NextCheckedDate,
                        dto.NextMileage,
                        carResult.Data.CurrentMileage
                    );

                    dto.CarStatus = calculated?.CarStatus ?? CarStatus.UnKnown;
                    dto.MileageDifference = calculated?.MileageDifference ?? 0;
                    dto.TimeDifference = calculated?.TimeDifference ?? TimeSpan.Zero;
                    dto.TimeDifferenceAsPercentage = calculated?.TimeDifferenceAsPercentage ?? 0;
                });

            // -----------------------------------------------------
            // 5️⃣ Date validations
            // -----------------------------------------------------
            RuleFor(x => x.NextCheckedDate)
                .GreaterThan(x => x.LastCheckedDate)
                .WithMessage("NextCheckedDate must be greater than LastCheckedDate.");

            // -----------------------------------------------------
            // 6️⃣ Mileage validations
            // -----------------------------------------------------
            RuleFor(x => x.NextMileage)
                .MustAsync(async (dto, nextMileage, ct) =>
                {
                    var carResult = await _carService.GetByIdAsync(dto.CarId);
                    if (carResult == null || carResult.Data == null)
                        return false;

                    return nextMileage > carResult.Data.CurrentMileage;
                })
                .WithMessage("NextMileage must be greater than the car's current mileage.");
        }
    }
}
