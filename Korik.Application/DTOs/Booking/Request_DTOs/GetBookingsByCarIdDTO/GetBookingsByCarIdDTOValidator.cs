using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class GetBookingsByCarIdDTOValidator : AbstractValidator<GetBookingsByCarIdDTO>
    {
        private readonly ICarService _carService;

        public GetBookingsByCarIdDTOValidator(ICarService carService)
        {
            _carService = carService;

            RuleFor(x => x.CarId)
                .GreaterThan(0)
                .WithMessage("Car Id must be greater than 0.")

                .MustAsync(async (carId, cancellationToken) =>
                {
                    var result = await _carService.IsExistAsync(carId);
                    return result.Data;       // true if exists, false if not
                })
                .WithMessage("Car does not exist.");
        }
    }

}
