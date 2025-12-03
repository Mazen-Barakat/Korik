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
        private readonly IWorkshopServiceService _workshopServiceService;

        public UpdateBookingDTOValidator(
            ICarService carService,
            IWorkShopProfileService workShopProfileService,
            IBookingService bookingService,
            IWorkshopServiceService workshopServiceService
            )
        {
            _carService = carService;
            _workShopProfileService = workShopProfileService;
            _bookingService = bookingService;
            _workshopServiceService = workshopServiceService;

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

            // ✅ SECURITY: Validate cancellation time window (12 hours from creation)
            RuleFor(x => x)
                .MustAsync(async (dto, cancellationToken) =>
                {
                    // Only validate if status is being changed to Cancelled
                    if (dto.Status != BookingStatus.Cancelled)
                        return true;

                    // Get the existing booking to check creation time
                    var bookingResult = await _bookingService.GetByIdAsync(dto.Id);
                    if (!bookingResult.Success || bookingResult.Data == null)
                        return false;

                    // Check if current status is already Cancelled (allow updates to cancelled bookings)
                    if (bookingResult.Data.Status == BookingStatus.Cancelled)
                        return true;

                    // Calculate time elapsed since booking creation
                    var timeElapsed = DateTime.UtcNow - bookingResult.Data.CreatedAt;
                    
                    // Allow cancellation only within 12 hours window
                    return timeElapsed.TotalHours <= 12;
                })
                .WithMessage("Booking cancellation is only allowed within 12 hours of creation. Please contact support for assistance.")
                .When(x => x.Status == BookingStatus.Cancelled);

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






