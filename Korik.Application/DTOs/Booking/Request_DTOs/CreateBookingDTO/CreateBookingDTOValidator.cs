using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class CreateBookingDTOValidator : AbstractValidator<CreateBookingDTO>
    {
        private readonly ICarService _carService;
        private readonly IWorkShopProfileService _workShopProfileService;
        private readonly IWorkshopServiceService _workshopServiceService;

        public CreateBookingDTOValidator(
            ICarService carService,
            IWorkShopProfileService workShopProfileService,
            IWorkshopServiceService workshopServiceService
            )
        {
            _carService = carService;
            _workShopProfileService = workShopProfileService;
            _workshopServiceService = workshopServiceService;


            RuleFor(x => x.AppointmentDate)
                .NotEmpty().WithMessage("Appointment date is required.")
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Appointment date must be in the future.");

            RuleFor(x => x.IssueDescription)
                .NotEmpty().WithMessage("Issue description is required.")
                .MinimumLength(10).WithMessage("Issue description must be at least 10 characters.")
                .MaximumLength(500).WithMessage("Issue description must not exceed 500 characters.");

            RuleFor(x => x.PaymentMethod)
                .IsInEnum().WithMessage("Invalid payment method.");

            RuleFor(x => x.PaidAmount)
                .GreaterThanOrEqualTo(0)
                .When(x => x.PaidAmount.HasValue)
                .WithMessage("Paid amount must be non-negative.");

            // ----- CarId validation -----
            RuleFor(x => x.CarId)
                .GreaterThan(0).WithMessage("CarId must be greater than 0.")
                .MustAsync(async (carId, _) =>
                {
                    var result = await _carService.IsExistAsync(carId);
                    return result.Data; // ServiceResult<bool>.Data
                })
                .WithMessage("Car does not exist.");

            // ----- WorkShopProfileId validation -----
            RuleFor(x => x.WorkShopProfileId)
                .GreaterThan(0).WithMessage("WorkShopProfileId must be greater than 0.")
                .MustAsync(async (wsId, _) =>
                {
                    var result = await _workShopProfileService.IsExistAsync(wsId);
                    return result.Data;
                })
                .WithMessage("Workshop profile does not exist.");

            // ----- WorkshopServiceId validation -----
            RuleFor(x => x.WorkshopServiceId)
                .GreaterThan(0).WithMessage("WorkshopServiceId must be greater than 0.")
                .MustAsync(async (serviceId, _) =>
                {
                    var result = await _workshopServiceService.IsExistAsync(serviceId);
                    return result.Data;
                })
                .WithMessage("Workshop service does not exist.");
        }
    }

}
