using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Korik.Domain;

namespace Korik.Application
{
    public class GetAllIndicatorByCarIdDTOValidator : AbstractValidator<GetAllIndicatorsByCarIdDTO>
    {
        private readonly ICarService _carService;

        public GetAllIndicatorByCarIdDTOValidator(ICarService carService)
        {
            _carService = carService;

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
