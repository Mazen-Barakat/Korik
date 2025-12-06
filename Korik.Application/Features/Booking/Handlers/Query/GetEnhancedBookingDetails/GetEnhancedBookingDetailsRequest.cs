using Korik.Domain;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record GetEnhancedBookingDetailsRequest(int BookingId, string ApplicationUserId) : IRequest<ServiceResult<EnhancedBookingDetailsDTO>> { }

    public class GetEnhancedBookingDetailsRequestHandler : IRequestHandler<GetEnhancedBookingDetailsRequest, ServiceResult<EnhancedBookingDetailsDTO>>
    {
        private readonly IBookingService _bookingService;

        public GetEnhancedBookingDetailsRequestHandler(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public async Task<ServiceResult<EnhancedBookingDetailsDTO>> Handle(GetEnhancedBookingDetailsRequest request, CancellationToken cancellationToken)
        {
            // Get booking with all related entities
            var bookingResult = await _bookingService.GetByIdWithIncludeAsync(
                request.BookingId,
                b => b.Car.CarOwnerProfile,
                b => b.WorkShopProfile,
                b => b.WorkshopService.Service
            );

            if (!bookingResult.Success || bookingResult.Data == null)
            {
                return ServiceResult<EnhancedBookingDetailsDTO>.Fail("Booking not found.");
            }

            var booking = bookingResult.Data;

            // Check authorization
            var carOwnerUserId = booking.Car.CarOwnerProfile.ApplicationUserId;
            var workshopOwnerUserId = booking.WorkShopProfile.ApplicationUserId;

            if (request.ApplicationUserId != carOwnerUserId && request.ApplicationUserId != workshopOwnerUserId)
            {
                return ServiceResult<EnhancedBookingDetailsDTO>.Fail("You are not authorized to view this booking.");
            }

            // Determine response status based on booking status
            int responseStatus = booking.Status switch
            {
                BookingStatus.Pending => 0,
                BookingStatus.Confirmed => 1,
                BookingStatus.Rejected => 2,
                BookingStatus.Cancelled => 2,
                _ => 1
            };

            // Can change response only if not accepted and time hasn't passed
            bool canChangeResponse = booking.Status == BookingStatus.Pending && DateTime.UtcNow < booking.AppointmentDate;

            var dto = new EnhancedBookingDetailsDTO
            {
                Id = booking.Id,
                BookingReference = $"BK-{booking.CreatedAt:yyyy}-{booking.Id:D6}",

                // Customer Info
                CustomerName = $"{booking.Car.CarOwnerProfile.FirstName} {booking.Car.CarOwnerProfile.LastName}",
                CustomerPhone = booking.Car.CarOwnerProfile.PhoneNumber ?? string.Empty,
                CustomerPhoto = booking.Car.CarOwnerProfile.ProfileImageUrl,

                // Vehicle Info
                VehicleInfo = $"{booking.Car.Year} {booking.Car.Make} {booking.Car.Model} - {booking.Car.LicensePlate}",
                VehicleMake = booking.Car.Make,
                VehicleModel = booking.Car.Model,
                VehicleYear = booking.Car.Year,
                VehiclePlateNumber = booking.Car.LicensePlate,

                // Service Info
                ServiceType = booking.WorkshopService.Service?.Name ?? "Service",
                ServiceList = new() { booking.WorkshopService.Service?.Name ?? "Service" },
                EstimatedDuration = booking.WorkshopService.Duration,
                EstimatedCost = booking.WorkshopService.MinPrice,

                // Appointment Info
                ExactAppointmentTime = booking.AppointmentDate,
                CreatedAt = booking.CreatedAt,

                // Workshop Info
                WorkshopName = booking.WorkShopProfile.Name,
                WorkshopAddress = $"{booking.WorkShopProfile.City}, {booking.WorkShopProfile.Governorate}, {booking.WorkShopProfile.Country}",
                WorkshopPhone = booking.WorkShopProfile.PhoneNumber,

                // Response Status
                ResponseStatus = responseStatus,
                CanChangeResponse = canChangeResponse,
                LastResponseChangedAt = null,
                ResponseChangedBy = null,

                // Confirmation Status
                CarOwnerConfirmed = booking.CarOwnerConfirmed,
                WorkshopConfirmed = booking.WorkshopOwnerConfirmed,

                // Notification Info
                NotificationType = (int)Domain.NotificationType.BookingCreated,
                Priority = "high",
                Status = booking.Status.ToString()
            };

            return ServiceResult<EnhancedBookingDetailsDTO>.Ok(dto, "Booking details retrieved successfully.");
        }
    }
}
