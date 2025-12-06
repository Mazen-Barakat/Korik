using Korik.Domain;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record UpdateBookingResponseRequest(int BookingId, UpdateBookingResponseDTO Model, string ApplicationUserId) 
        : IRequest<ServiceResult<BookingResponseResultDTO>> { }

    public class UpdateBookingResponseRequestHandler : IRequestHandler<UpdateBookingResponseRequest, ServiceResult<BookingResponseResultDTO>>
    {
        private readonly IBookingService _bookingService;
        private readonly INotificationService _notificationService;

        public UpdateBookingResponseRequestHandler(
            IBookingService bookingService,
            INotificationService notificationService)
        {
            _bookingService = bookingService;
            _notificationService = notificationService;
        }

        public async Task<ServiceResult<BookingResponseResultDTO>> Handle(UpdateBookingResponseRequest request, CancellationToken cancellationToken)
        {
            // Get booking with related entities
            var bookingResult = await _bookingService.GetByIdWithIncludeAsync(
                request.BookingId,
                b => b.Car.CarOwnerProfile,
                b => b.WorkShopProfile
            );

            if (!bookingResult.Success || bookingResult.Data == null)
            {
                return ServiceResult<BookingResponseResultDTO>.Fail("Booking not found.");
            }

            var booking = bookingResult.Data;

            // Check authorization
            var carOwnerUserId = booking.Car.CarOwnerProfile.ApplicationUserId;
            var workshopOwnerUserId = booking.WorkShopProfile.ApplicationUserId;

            bool isWorkshop = request.ApplicationUserId == workshopOwnerUserId;
            bool isCustomer = request.ApplicationUserId == carOwnerUserId;

            if (!isWorkshop && !isCustomer)
            {
                return ServiceResult<BookingResponseResultDTO>.Fail("You are not authorized to update this booking.");
            }

            // Get current response status
            int currentResponseStatus = booking.Status switch
            {
                BookingStatus.Pending => 0,
                BookingStatus.Confirmed => 1,
                BookingStatus.Rejected => 2,
                BookingStatus.Cancelled => 2,
                _ => 1
            };

            int requestedStatus = request.Model.ResponseStatus;

            // Business Rule 1: Check if time has passed
            if (DateTime.UtcNow > booking.AppointmentDate)
            {
                return ServiceResult<BookingResponseResultDTO>.Fail("Cannot change response after appointment time has passed.");
            }

            // Business Rule 2: Acceptance is final
            if (currentResponseStatus == 1 && requestedStatus != 1)
            {
                return ServiceResult<BookingResponseResultDTO>.Fail("Cannot change from Accepted status (acceptance is final).");
            }

            // Update booking status based on response
            BookingStatus newBookingStatus = requestedStatus switch
            {
                0 => BookingStatus.Pending,
                1 => BookingStatus.Confirmed,
                2 => BookingStatus.Rejected,
                _ => booking.Status
            };

            var previousStatus = currentResponseStatus;
            booking.Status = newBookingStatus;

            // Save changes
            var updateResult = await _bookingService.UpdateAsync(booking);
            if (!updateResult.Success)
            {
                return ServiceResult<BookingResponseResultDTO>.Fail("Failed to update booking response.");
            }

            // Determine who changed and who to notify
            string receiverId = isWorkshop ? carOwnerUserId : workshopOwnerUserId;
            string senderId = isWorkshop ? workshopOwnerUserId : carOwnerUserId;
            string changedBy = isWorkshop ? "workshop" : "customer";
            string bookingReference = $"BK-{booking.CreatedAt:yyyy}-{booking.Id:D6}";

            // Send notification to the other party
            string message = requestedStatus switch
            {
                1 => $"Booking has been accepted by {changedBy}.",
                2 => $"Booking has been declined by {changedBy}.",
                _ => $"Booking status has been updated by {changedBy}."
            };

            await _notificationService.SendNotificationAsync(
                senderId,
                receiverId,
                message,
                NotificationType.ResponseStatusChanged,
                booking.Id,
                "Booking Response Updated",
                "high"
            );

            // Send SignalR event for real-time update
            await _notificationService.SendBookingResponseChangedAsync(
                receiverId,
                booking.Id,
                bookingReference,
                previousStatus,
                requestedStatus,
                changedBy
            );

            // Determine if response can be changed again
            bool canChangeAgain = requestedStatus != 1; // Can't change if accepted

            var result = new BookingResponseResultDTO
            {
                BookingId = booking.Id,
                BookingReference = bookingReference,
                PreviousStatus = previousStatus,
                NewStatus = requestedStatus,
                CanChangeAgain = canChangeAgain,
                LastResponseChangedAt = DateTime.UtcNow,
                ResponseChangedBy = changedBy
            };

            return ServiceResult<BookingResponseResultDTO>.Ok(result, "Booking response updated successfully.");
        }
    }
}
