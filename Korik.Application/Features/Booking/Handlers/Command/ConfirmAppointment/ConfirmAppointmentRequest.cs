using AutoMapper;
using FluentValidation;
using Korik.Domain;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record ConfirmAppointmentRequest(ConfirmAppointmentDTO Model) : IRequest<ServiceResult<BookingDTO>> { }

    public class ConfirmAppointmentRequestHandler : IRequestHandler<ConfirmAppointmentRequest, ServiceResult<BookingDTO>>
    {
        private readonly IBookingService _bookingService;
        private readonly ICarService _carService;
        private readonly IWorkShopProfileService _workshopService;
        private readonly INotificationService _notificationService;
        private readonly IValidator<ConfirmAppointmentDTO> _validator;
        private readonly IMapper _mapper;

        public ConfirmAppointmentRequestHandler(
            IBookingService bookingService,
            ICarService carService,
            IWorkShopProfileService workshopService,
            INotificationService notificationService,
            IValidator<ConfirmAppointmentDTO> validator,
            IMapper mapper)
        {
            _bookingService = bookingService;
            _carService = carService;
            _workshopService = workshopService;
            _notificationService = notificationService;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<ServiceResult<BookingDTO>> Handle(ConfirmAppointmentRequest request, CancellationToken cancellationToken)
        {
            // Validate request
            var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<BookingDTO>.Fail(errors);
            }

            // Get booking with related entities
            var bookingResult = await _bookingService.GetByIdWithIncludeAsync(
                request.Model.BookingId,
                b => b.Car.CarOwnerProfile,
                b => b.WorkShopProfile
            );

            if (!bookingResult.Success || bookingResult.Data == null)
                return ServiceResult<BookingDTO>.Fail("Booking not found.");

            var booking = bookingResult.Data;

            if (booking.Status != BookingStatus.Confirmed)
                return ServiceResult<BookingDTO>.Fail("Booking is not in Confirmed status.");

            if (booking.ConfirmationDeadline.HasValue && DateTime.UtcNow > booking.ConfirmationDeadline.Value)
                return ServiceResult<BookingDTO>.Fail("Confirmation deadline has passed.");

            var carOwnerUserId = booking.Car.CarOwnerProfile.ApplicationUserId;
            var workshopOwnerUserId = booking.WorkShopProfile.ApplicationUserId;

            bool isCarOwner = request.Model.ApplicationUserId == carOwnerUserId;
            bool isWorkshopOwner = request.Model.ApplicationUserId == workshopOwnerUserId;

            if (!isCarOwner && !isWorkshopOwner)
                return ServiceResult<BookingDTO>.Fail("You are not authorized to confirm this appointment.");

            // Only process if user is confirming (isConfirmed = true)
            // "Close" button on frontend no longer calls API, so we only handle confirmations
            if (!request.Model.IsConfirmed)
            {
                // If someone explicitly declines via API, just return current state
                // The dialog close action doesn't call API anymore
                var currentDto = _mapper.Map<BookingDTO>(booking);
                return ServiceResult<BookingDTO>.Ok(currentDto, "Confirmation declined. Waiting for timeout or confirmation.");
            }

            // Update confirmation status
            if (isCarOwner)
                booking.CarOwnerConfirmed = true;
            else if (isWorkshopOwner)
                booking.WorkshopOwnerConfirmed = true;

            // Check if both parties have now confirmed
            await ProcessConfirmationResponse(booking, carOwnerUserId, workshopOwnerUserId, isCarOwner);

            var updateResult = await _bookingService.UpdateAsync(booking);
            if (!updateResult.Success)
                return ServiceResult<BookingDTO>.Fail("Failed to update booking.");

            var bookingDto = _mapper.Map<BookingDTO>(updateResult.Data);
            
            string message = (booking.CarOwnerConfirmed == true && booking.WorkshopOwnerConfirmed == true)
                ? "Both parties confirmed! Appointment is now in progress."
                : "Confirmation recorded. Waiting for the other party to confirm.";
            
            return ServiceResult<BookingDTO>.Ok(bookingDto, message);
        }

        private async Task ProcessConfirmationResponse(Booking booking, string carOwnerUserId, string workshopOwnerUserId, bool isCarOwner)
        {
            // Check if BOTH parties have confirmed
            if (booking.CarOwnerConfirmed == true && booking.WorkshopOwnerConfirmed == true)
            {
                // Both confirmed - Move to InProgress
                booking.Status = BookingStatus.InProgress;

                booking.ConfirmationSentAt = DateTime.UtcNow; // Clear sent time as both have confirmed    

                booking.ConfirmationDeadline = DateTime.UtcNow.AddMinutes(15); // Clear deadline as both have confirmed   
                // Send real-time update to BOTH to dismiss dialogs
                await _notificationService.SendConfirmationStatusUpdateAsync(
                    carOwnerUserId, booking.Id,
                    booking.CarOwnerConfirmed, booking.WorkshopOwnerConfirmed,
                    BookingStatus.InProgress.ToString(), shouldDismissDialog: true);
                
                await _notificationService.SendConfirmationStatusUpdateAsync(
                    workshopOwnerUserId, booking.Id,
                    booking.CarOwnerConfirmed, booking.WorkshopOwnerConfirmed,
                    BookingStatus.InProgress.ToString(), shouldDismissDialog: true);

                // Send success notifications to both parties
                await SendNotificationToBothParties(
                    booking, carOwnerUserId, workshopOwnerUserId,
                    "Appointment confirmed! Service is now in progress.",
                    NotificationType.BookingAccepted,
                    "Appointment Started", "high");
            }
            else
            {
                // One party confirmed - notify the other party to confirm
                string receiverUserId = isCarOwner ? workshopOwnerUserId : carOwnerUserId;
                string senderUserId = isCarOwner ? carOwnerUserId : workshopOwnerUserId;

                // Send real-time update to show the other party that one has confirmed
                await _notificationService.SendConfirmationStatusUpdateAsync(
                    receiverUserId, booking.Id,
                    booking.CarOwnerConfirmed, booking.WorkshopOwnerConfirmed,
                    booking.Status.ToString(), shouldDismissDialog: false);

                // Send notification to the other party
                string message = isCarOwner
                    ? "Car owner has confirmed arrival. Please confirm your readiness."
                    : "Workshop is ready to receive you. Please confirm your arrival.";

                await _notificationService.SendNotificationAsync(
                    senderUserId, receiverUserId, message,
                    NotificationType.AppointmentConfirmationRequest, booking.Id,
                    "Waiting for Your Confirmation", "high");
            }
        }

        private async Task SendNotificationToBothParties(Booking booking, string carOwnerUserId, string workshopOwnerUserId,
            string message, NotificationType type, string title, string priority)
        {
            await _notificationService.SendNotificationAsync(workshopOwnerUserId, carOwnerUserId, message, type, booking.Id, title, priority);
            await _notificationService.SendNotificationAsync(carOwnerUserId, workshopOwnerUserId, message, type, booking.Id, title, priority);
        }
    }
}
