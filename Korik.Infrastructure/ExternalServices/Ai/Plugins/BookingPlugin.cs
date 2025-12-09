using Korik.Application;
using Korik.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text;

namespace Korik.Infrastructure
{
    public class BookingPlugin
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICarOwnerProfileRepository _carOwnerProfileRepository;
        private readonly IWorkShopProfileRepository _workShopProfileRepository;

        public BookingPlugin(
            IBookingRepository bookingRepository,
            ICarOwnerProfileRepository carOwnerProfileRepository,
            IWorkShopProfileRepository workShopProfileRepository)
        {
            _bookingRepository = bookingRepository;
            _carOwnerProfileRepository = carOwnerProfileRepository;
            _workShopProfileRepository = workShopProfileRepository;
        }

        [KernelFunction("GetMyBookings")]
        [Description("Gets all bookings for the current user. Use this when user asks about their bookings, appointments, or service history.")]
        public async Task<string> GetMyBookingsAsync(
            [Description("The authenticated user's ID")] string userId)
        {
            var carOwnerProfile = await _carOwnerProfileRepository.GetByApplicationUserIdAsync(userId);

            if (carOwnerProfile == null)
            {
                return "You don't have a car owner profile yet. Please create one first.";
            }

            var bookings = await _bookingRepository.GetAllWithIncludeAsync(
                    b => b.Car,
                    b => b.WorkShopProfile,
                    b => b.WorkshopService,
                    b => b.WorkshopService.Service)
                ?.Where(b => b.Car.CarOwnerProfileId == carOwnerProfile.Id)
                .OrderByDescending(b => b.AppointmentDate)
                .Take(10)
                .ToListAsync() ?? [];

            if (bookings.Count == 0)
            {
                return "You don't have any bookings yet.";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"You have {bookings.Count} recent bookings:");

            foreach (var booking in bookings)
            {
                sb.AppendLine($"- Booking #{booking.Id}: {booking.WorkshopService?.Service?.Name ?? "Service"} at {booking.WorkShopProfile?.Name ?? "Workshop"}");
                sb.AppendLine($"  Car: {booking.Car?.Make} {booking.Car?.Model} ({booking.Car?.LicensePlate})");
                sb.AppendLine($"  Date: {booking.AppointmentDate:yyyy-MM-dd HH:mm}");
                sb.AppendLine($"  Status: {booking.Status}");
                sb.AppendLine($"  Payment: {booking.PaymentStatus} ({booking.PaymentMethod})");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        [KernelFunction("GetBookingDetails")]
        [Description("Gets details of a specific booking by its ID. Use this when user asks about a particular booking.")]
        public async Task<string> GetBookingDetailsAsync(
            [Description("The booking ID to retrieve")] int bookingId,
            [Description("The authenticated user's ID")] string userId)
        {
            var booking = await _bookingRepository.GetByIdWithIncludeAsync(
                bookingId,
                b => b.Car,
                b => b.WorkShopProfile,
                b => b.WorkshopService,
                b => b.WorkshopService.Service,
                b => b.Review);

            if (booking == null)
            {
                return $"Booking #{bookingId} was not found.";
            }

            // Verify the booking belongs to the user
            var carOwnerProfile = await _carOwnerProfileRepository.GetByApplicationUserIdAsync(userId);
            if (carOwnerProfile == null || booking.Car?.CarOwnerProfileId != carOwnerProfile.Id)
            {
                return "You don't have access to this booking.";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"Booking #{booking.Id} Details:");
            sb.AppendLine($"Service: {booking.WorkshopService?.Service?.Name ?? "N/A"}");
            sb.AppendLine($"Workshop: {booking.WorkShopProfile?.Name ?? "N/A"}");
            sb.AppendLine($"Car: {booking.Car?.Make} {booking.Car?.Model} ({booking.Car?.Year})");
            sb.AppendLine($"License Plate: {booking.Car?.LicensePlate}");
            sb.AppendLine($"Appointment Date: {booking.AppointmentDate:yyyy-MM-dd HH:mm}");
            sb.AppendLine($"Status: {booking.Status}");
            sb.AppendLine($"Issue Description: {booking.IssueDescription ?? "None provided"}");
            sb.AppendLine($"Payment Method: {booking.PaymentMethod}");
            sb.AppendLine($"Payment Status: {booking.PaymentStatus}");

            if (booking.PaidAmount.HasValue)
            {
                sb.AppendLine($"Amount Paid: {booking.PaidAmount:C}");
            }

            if (booking.Review != null)
            {
                sb.AppendLine($"Your Review: {booking.Review.Rating}/5 stars - \"{booking.Review.Comment}\"");
            }

            return sb.ToString();
        }

        [KernelFunction("GetUpcomingBookings")]
        [Description("Gets all upcoming/future bookings for the user. Use this when user asks about their upcoming appointments or scheduled services.")]
        public async Task<string> GetUpcomingBookingsAsync(
            [Description("The authenticated user's ID")] string userId)
        {
            var carOwnerProfile = await _carOwnerProfileRepository.GetByApplicationUserIdAsync(userId);

            if (carOwnerProfile == null)
            {
                return "You don't have a car owner profile yet.";
            }

            var now = DateTime.UtcNow;
            var bookings = await _bookingRepository.GetAllWithIncludeAsync(
                    b => b.Car,
                    b => b.WorkShopProfile,
                    b => b.WorkshopService,
                    b => b.WorkshopService.Service)
                ?.Where(b => b.Car.CarOwnerProfileId == carOwnerProfile.Id
                    && b.AppointmentDate > now
                    && b.Status != BookingStatus.Cancelled
                    && b.Status != BookingStatus.Completed
                    && b.Status != BookingStatus.Rejected)
                .OrderBy(b => b.AppointmentDate)
                .ToListAsync() ?? [];

            if (bookings.Count == 0)
            {
                return "You don't have any upcoming bookings.";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"You have {bookings.Count} upcoming booking(s):");

            foreach (var booking in bookings)
            {
                var daysUntil = (booking.AppointmentDate - now).Days;
                sb.AppendLine($"- {booking.WorkshopService?.Service?.Name} at {booking.WorkShopProfile?.Name}");
                sb.AppendLine($"  Date: {booking.AppointmentDate:yyyy-MM-dd HH:mm} ({(daysUntil == 0 ? "Today" : $"in {daysUntil} days")})");
                sb.AppendLine($"  Car: {booking.Car?.Make} {booking.Car?.Model}");
                sb.AppendLine($"  Status: {booking.Status}");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        [KernelFunction("GetBookingHistory")]
        [Description("Gets completed booking history for the user. Use this when user asks about past services or completed bookings.")]
        public async Task<string> GetBookingHistoryAsync(
            [Description("The authenticated user's ID")] string userId)
        {
            var carOwnerProfile = await _carOwnerProfileRepository.GetByApplicationUserIdAsync(userId);

            if (carOwnerProfile == null)
            {
                return "You don't have a car owner profile yet.";
            }

            var bookings = await _bookingRepository.GetAllWithIncludeAsync(
                    b => b.Car,
                    b => b.WorkShopProfile,
                    b => b.WorkshopService,
                    b => b.WorkshopService.Service,
                    b => b.Review)
                ?.Where(b => b.Car.CarOwnerProfileId == carOwnerProfile.Id
                    && b.Status == BookingStatus.Completed)
                .OrderByDescending(b => b.AppointmentDate)
                .Take(15)
                .ToListAsync() ?? [];

            if (bookings.Count == 0)
            {
                return "You don't have any completed bookings in your history.";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"Your completed service history ({bookings.Count} bookings):");

            foreach (var booking in bookings)
            {
                sb.AppendLine($"- {booking.WorkshopService?.Service?.Name} at {booking.WorkShopProfile?.Name}");
                sb.AppendLine($"  Date: {booking.AppointmentDate:yyyy-MM-dd}");
                sb.AppendLine($"  Car: {booking.Car?.Make} {booking.Car?.Model}");
                if (booking.PaidAmount.HasValue)
                {
                    sb.AppendLine($"  Cost: {booking.PaidAmount:C}");
                }
                if (booking.Review != null)
                {
                    sb.AppendLine($"  Rating: {booking.Review.Rating}/5 stars");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        [KernelFunction("GetWorkshopBookings")]
        [Description("Gets all bookings for a workshop owner. Use this when a workshop owner asks about their received bookings.")]
        public async Task<string> GetWorkshopBookingsAsync(
            [Description("The authenticated user's ID")] string userId,
            [Description("Filter by status (optional): Pending, Confirmed, InProgress, Completed, Cancelled")] string? statusFilter = null)
        {
            var workshopProfile = await _workShopProfileRepository.GetByApplicationUserIdAsync(userId);

            if (workshopProfile == null)
            {
                return "You don't have a workshop profile. This function is for workshop owners.";
            }

            var query = _bookingRepository.GetBookingsByWorkshopProfileIdAsync(workshopProfile.Id)
                .Include(b => b.Car)
                .Include(b => b.WorkshopService)
                .ThenInclude(ws => ws.Service);

            IQueryable<Booking> filteredQuery = query;

            if (!string.IsNullOrEmpty(statusFilter) && Enum.TryParse<BookingStatus>(statusFilter, true, out var status))
            {
                filteredQuery = query.Where(b => b.Status == status);
            }

            var bookings = await filteredQuery
                .OrderByDescending(b => b.AppointmentDate)
                .Take(20)
                .ToListAsync();

            if (bookings.Count == 0)
            {
                return statusFilter != null
                    ? $"No bookings found with status '{statusFilter}'."
                    : "You don't have any bookings at your workshop.";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"Workshop bookings ({bookings.Count}):");

            foreach (var booking in bookings)
            {
                sb.AppendLine($"- Booking #{booking.Id}: {booking.WorkshopService?.Service?.Name ?? "Service"}");
                sb.AppendLine($"  Car: {booking.Car?.Make} {booking.Car?.Model} ({booking.Car?.LicensePlate})");
                sb.AppendLine($"  Date: {booking.AppointmentDate:yyyy-MM-dd HH:mm}");
                sb.AppendLine($"  Status: {booking.Status}");
                sb.AppendLine($"  Issue: {booking.IssueDescription ?? "Not specified"}");
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
