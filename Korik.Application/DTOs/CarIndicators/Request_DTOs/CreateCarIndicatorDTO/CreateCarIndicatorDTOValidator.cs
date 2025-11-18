using FluentValidation;
using Korik.Domain;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class CreateCarIndicatorDTOValidator : AbstractValidator<CreateCarIndicatorDTO>
    {
        private readonly ICarService _carService;
        private readonly ICarIndicatorStatusService _statusService;

        public CreateCarIndicatorDTOValidator(
            ICarService carService,
            ICarIndicatorStatusService statusService)
        {
            _carService = carService;
            _statusService = statusService;

            // -------------------------------
            // 1️. Car must exist
            // -------------------------------
            RuleFor(x => x.CarId)
                .MustAsync(async (carId, ct) =>
                {
                    var result = await _carService.IsExistAsync(carId);
                    // Return false if service failed or car doesn't exist
                    return result.Success && result.Data;
                })
                .WithMessage("The specified car does not exist.");

            // -------------------------------
            // 2️. Automatically calculate all business fields
            // -------------------------------
            RuleFor(x => x)
             .CustomAsync(async (dto, context, ct) =>
             {
                 var carResult = await _carService.GetByIdAsync(dto.CarId);
                 if (carResult == null || carResult.Data == null)
                 {
                     context.AddFailure("Car not found.");
                     return;
                 }

                 // Ensure dates are valid
                 if (dto.LastCheckedDate == default || dto.NextCheckedDate == default)
                 {
                     context.AddFailure("LastCheckedDate and NextCheckedDate must be provided.");
                     return;
                 }

                 var calculated = _statusService.CalculateAll(
                     dto.IndicatorType,
                     dto.LastCheckedDate,
                     dto.NextCheckedDate,
                     dto.NextMileage,
                     carResult.Data.CurrentMileage
                 );

                 // Safely assign calculated values
                 dto.CarStatus = calculated?.CarStatus ?? CarStatus.UnKnown;
                 dto.MileageDifference = calculated?.MileageDifference ?? 0;
                 dto.TimeDifference = calculated?.TimeDifference ?? TimeSpan.Zero; ;
                 dto.TimeDifferenceAsPercentage = calculated?.TimeDifferenceAsPercentage ?? 0;
             });


            // -------------------------------
            // 3️. Additional simple validations
            // -------------------------------
            RuleFor(x => x.NextCheckedDate)
                .GreaterThan(x => x.LastCheckedDate)
                .WithMessage("NextCheckedDate must be greater than LastCheckedDate.");



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
