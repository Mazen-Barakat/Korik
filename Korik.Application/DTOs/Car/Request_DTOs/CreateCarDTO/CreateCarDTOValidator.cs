using System;
using FluentValidation;

namespace Korik.Application
{
    public class CreateCarDTOValidator : AbstractValidator<CreateCarDTO>
    {
        private readonly ICarService _carService;
        private readonly ICarOwnerProfileService _carOwnerProfileService;

        public CreateCarDTOValidator(ICarService carService, ICarOwnerProfileService carOwnerProfileService)
        {
            _carService = carService;
            _carOwnerProfileService = carOwnerProfileService;

            // Make
            RuleFor(x => x.Make)
                .NotEmpty().WithMessage("Make is required.")
                .MaximumLength(100).WithMessage("Make cannot exceed 100 characters.");

            // Model
            RuleFor(x => x.Model)
                .NotEmpty().WithMessage("Model is required.")
                .MaximumLength(100).WithMessage("Model cannot exceed 100 characters.");

            // Year (from 1960 to next year)
            var currentYear = DateTime.UtcNow.Year + 1;
            RuleFor(x => x.Year)
                .InclusiveBetween(1960, currentYear)
                .WithMessage($"Year must be between 1960 and {currentYear}.");

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
                .MustAsync(async (licensePlate, cancellation) =>
                {
                    // Check if the license plate exists in the database
                    var isLicensePlateInUse = await _carService.GetByLicensePlateAsync(licensePlate, 0);

                    // Ensure the license plate is unique
                    return !isLicensePlateInUse.Data;
                }).WithMessage("LicensePlate must be unique.");

            // Enums
            RuleFor(x => x.TransmissionType)
                .IsInEnum().WithMessage("Invalid TransmissionType.");

            RuleFor(x => x.FuelType)
                .IsInEnum().WithMessage("Invalid FuelType.");

            RuleFor(x => x.Origin)
                .IsInEnum().WithMessage("Invalid Car Origin");

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
