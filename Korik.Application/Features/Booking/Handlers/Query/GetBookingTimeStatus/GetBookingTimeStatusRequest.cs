using Korik.Domain;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record GetBookingTimeStatusRequest(int BookingId, string ApplicationUserId) : IRequest<ServiceResult<BookingTimeStatusDTO>> { }

    public class GetBookingTimeStatusRequestHandler : IRequestHandler<GetBookingTimeStatusRequest, ServiceResult<BookingTimeStatusDTO>>
    {
        private readonly IBookingService _bookingService;

        public GetBookingTimeStatusRequestHandler(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public async Task<ServiceResult<BookingTimeStatusDTO>> Handle(GetBookingTimeStatusRequest request, CancellationToken cancellationToken)
        {
            // Get booking with related entities
            var bookingResult = await _bookingService.GetByIdWithIncludeAsync(
                request.BookingId,
                b => b.Car.CarOwnerProfile,
                b => b.WorkShopProfile
            );

            if (!bookingResult.Success || bookingResult.Data == null)
            {
                return ServiceResult<BookingTimeStatusDTO>.Fail("Booking not found.");
            }

            var booking = bookingResult.Data;

            // Check authorization
            var carOwnerUserId = booking.Car.CarOwnerProfile.ApplicationUserId;
            var workshopOwnerUserId = booking.WorkShopProfile.ApplicationUserId;

            if (request.ApplicationUserId != carOwnerUserId && request.ApplicationUserId != workshopOwnerUserId)
            {
                return ServiceResult<BookingTimeStatusDTO>.Fail("You are not authorized to view this booking.");
            }

            var now = DateTime.UtcNow;
            var secondsUntilArrival = (int)(booking.AppointmentDate - now).TotalSeconds;
            var hasArrived = secondsUntilArrival <= 0;

            // Determine response status
            int responseStatus = booking.Status switch
            {
                BookingStatus.Pending => 0,
                BookingStatus.Confirmed => 1,
                BookingStatus.Rejected => 2,
                BookingStatus.Cancelled => 2,
                _ => 1
            };

            // Can still change response if time hasn't passed and booking is pending/declined
            bool canStillChangeResponse = !hasArrived && 
                (booking.Status == BookingStatus.Pending || booking.Status == BookingStatus.Rejected);

            var dto = new BookingTimeStatusDTO
            {
                BookingId = booking.Id,
                BookingReference = $"BK-{booking.CreatedAt:yyyy}-{booking.Id:D6}",
                ExactAppointmentTime = booking.AppointmentDate,
                CurrentTime = now,
                HasArrived = hasArrived,
                SecondsUntilArrival = secondsUntilArrival,
                CanStillChangeResponse = canStillChangeResponse,
                ResponseStatus = responseStatus,
                Status = booking.Status.ToString()
            };

            return ServiceResult<BookingTimeStatusDTO>.Ok(dto, "Time status retrieved successfully.");
        }
    }
}
