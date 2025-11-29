using FluentValidation;
using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{

    using FluentValidation;

    public class UpdateBookingDTOValidator : AbstractValidator<UpdateBookingDTO>
    {
        private readonly ICarService _carService;
        private readonly IWorkShopProfileService _workShopProfileService;
        private readonly IBookingService _bookingService;

        public UpdateBookingDTOValidator(
            ICarService carService,
            IWorkShopProfileService workShopProfileService,
            IBookingService bookingService
            )
        {
            _carService = carService;
            _workShopProfileService = workShopProfileService;
            _bookingService = bookingService;

            // ---- ID CHECK USING BOOKING SERVICE ----
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Booking Id must be greater than 0.")
                .MustAsync(async (id, _) =>
                {
                    var result = await _bookingService.IsExistAsync(id);
                    return result.Data;
                })
                .WithMessage("Booking does not exist.");


            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Invalid booking status.");

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

            RuleFor(x => x.PaymentStatus)
                .IsInEnum().WithMessage("Invalid payment status.");

            RuleFor(x => x.PaidAmount)
                .GreaterThanOrEqualTo(0)
                .When(x => x.PaidAmount.HasValue)
                .WithMessage("Paid amount cannot be negative.");

            // -------- CarId async validation --------
            When(x => x.CarId.HasValue, () =>
            {
                RuleFor(x => x.CarId.Value)
                    .GreaterThan(0).WithMessage("CarId must be greater than 0.")
                    .MustAsync(async (carId, _) =>
                    {
                        var result = await _carService.IsExistAsync(carId);
                        return result.Data;
                    })
                    .WithMessage("Car does not exist.");
            });

            // -------- WorkShopProfileId async validation --------
            When(x => x.WorkShopProfileId.HasValue, () =>
            {
                RuleFor(x => x.WorkShopProfileId.Value)
                    .GreaterThan(0).WithMessage("WorkShopProfileId must be greater than 0.")
                    .MustAsync(async (wsId, _) =>
                    {
                        var result = await _workShopProfileService.IsExistAsync(wsId);
                        return result.Data;
                    })
                    .WithMessage("Workshop profile does not exist.");
            });

            // -------- WorkshopServiceId async validation --------
            When(x => x.WorkshopServiceId.HasValue, () =>
            {
                RuleFor(x => x.WorkshopServiceId.Value)
                    .GreaterThan(0).WithMessage("WorkshopServiceId must be greater than 0.")
                    .MustAsync(async (serviceId, _) =>
                    {
                        var result = await _workshopServiceService.IsExistAsync(serviceId);
                        return result.Data;
                    })
                    .WithMessage("Workshop service does not exist.");
            });
        }
    }


}






