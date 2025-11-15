using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class UpdateCarDTOValidator : AbstractValidator<UpdateCarDTO>
    {
        private readonly ICarService _carService;
        private readonly ICarOwnerProfileService _carOwnerProfileService;

        public UpdateCarDTOValidator(ICarService carService, ICarOwnerProfileService carOwnerProfileService)
        {
            _carService = carService;
            _carOwnerProfileService = carOwnerProfileService;

            // Id
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than 0.");

            // Make
            RuleFor(x => x.Make)
                .NotEmpty().WithMessage("Make is required.")
                .MaximumLength(100).WithMessage("Make cannot exceed 100 characters.");

            // Model
            RuleFor(x => x.Model)
                .NotEmpty().WithMessage("Model is required.")
                .MaximumLength(100).WithMessage("Model cannot exceed 100 characters.");

            // Year (from first car invention to next year)
            var currentYear = DateTime.UtcNow.Year;
            RuleFor(x => x.Year)
                .InclusiveBetween(1886, currentYear + 1)
                .WithMessage($"Year must be between 1886 and {currentYear + 1}.");

            // Engine capacity (positive)
            RuleFor(x => x.EngineCapacity)
                .GreaterThan(0).WithMessage("EngineCapacity must be greater than 0.");

            // Current mileage (non-negative)
            RuleFor(x => x.CurrentMileage)
                .GreaterThanOrEqualTo(0).WithMessage("CurrentMileage must be non-negative.");

            // License plate
            RuleFor(x => x.LicensePlate)
                .NotEmpty().WithMessage("LicensePlate is required.")
                .MaximumLength(20).WithMessage("LicensePlate cannot exceed 20 characters.")
                .MustAsync(async (dto, licensePlate, cancellation) =>
                {
                    // Check if the license plate exists in the database and does not belong to the current car
                    var isLicensePlateInUse = await _carService.GetByLicensePlateAsync(licensePlate, dto.Id);

                    // Allow the same license plate for the car being updated or a new unique license plate
                    return !isLicensePlateInUse.Data;
                }).WithMessage("The LicensePlate is already in use by another car owner.");

            // Enums
            RuleFor(x => x.TransmissionType)
                .IsInEnum().WithMessage("Invalid TransmissionType.");

            RuleFor(x => x.FuelType)
                .IsInEnum().WithMessage("Invalid FuelType.");

            // Foreign key
            RuleFor(x => x.CarOwnerProfileId)
                .GreaterThan(0).WithMessage("CarOwnerProfileId is required and must be greater than 0.")
                .MustAsync(async (carOwnerProfileId, cancellation) =>
                {
                    var result = await _carOwnerProfileService.GetByIdAsync(carOwnerProfileId);
                    return result.Success && result.Data != null;
                }).WithMessage("The specified CarOwnerProfileId does not exist.");
        }
    }
}
