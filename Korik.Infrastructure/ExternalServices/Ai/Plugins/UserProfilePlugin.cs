using Korik.Application;
using Korik.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text;

namespace Korik.Infrastructure
{
    /// <summary>
    /// Plugin for handling user profile queries via AI assistant.
    /// </summary>
    public class UserProfilePlugin
    {
        private readonly ICarOwnerProfileRepository _carOwnerProfileRepository;
        private readonly IWorkShopProfileRepository _workShopProfileRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICarRepository _carRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IReviewRepository _reviewRepository;

        public UserProfilePlugin(
            ICarOwnerProfileRepository carOwnerProfileRepository,
            IWorkShopProfileRepository workShopProfileRepository,
            UserManager<ApplicationUser> userManager,
            ICarRepository carRepository,
            IBookingRepository bookingRepository,
            IReviewRepository reviewRepository)
        {
            _carOwnerProfileRepository = carOwnerProfileRepository;
            _workShopProfileRepository = workShopProfileRepository;
            _userManager = userManager;
            _carRepository = carRepository;
            _bookingRepository = bookingRepository;
            _reviewRepository = reviewRepository;
        }

        [KernelFunction("GetMyProfile")]
        [Description("Gets the current user's profile information. Use this when user asks about their profile, account, or personal information.")]
        public async Task<string> GetMyProfileAsync(
            [Description("The authenticated user's ID")] string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return "User account not found.";
            }

            var carOwnerProfile = await _carOwnerProfileRepository.GetByApplicationUserIdAsync(userId);
            var workshopProfile = await _workShopProfileRepository.GetByApplicationUserIdAsync(userId);

            var sb = new StringBuilder();
            sb.AppendLine("?? Your Profile Information:");
            sb.AppendLine();

            sb.AppendLine("?? Account Details:");
            sb.AppendLine($"   Email: {user.Email}");
            sb.AppendLine($"   Phone: {user.PhoneNumber ?? "Not set"}");
            sb.AppendLine($"   Member Since: {user.CreatedAt:yyyy-MM-dd}");
            sb.AppendLine();

            if (carOwnerProfile != null)
            {
                sb.AppendLine("?? Car Owner Profile:");
                sb.AppendLine($"   Name: {carOwnerProfile.FirstName} {carOwnerProfile.LastName}");
                sb.AppendLine($"   Phone: {carOwnerProfile.PhoneNumber}");
                sb.AppendLine($"   Location: {carOwnerProfile.City}, {carOwnerProfile.Governorate}, {carOwnerProfile.Country}");
                sb.AppendLine($"   Preferred Language: {carOwnerProfile.PreferredLanguage}");
                sb.AppendLine();
            }

            if (workshopProfile != null)
            {
                sb.AppendLine("?? Workshop Owner Profile:");
                sb.AppendLine($"   Workshop: {workshopProfile.Name}");
                sb.AppendLine($"   Type: {workshopProfile.WorkShopType}");
                sb.AppendLine($"   Rating: ? {workshopProfile.Rating:F1}/5");
                sb.AppendLine($"   Verification: {workshopProfile.VerificationStatus}");
                sb.AppendLine();
            }

            if (carOwnerProfile == null && workshopProfile == null)
            {
                sb.AppendLine("?? You haven't created a profile yet.");
            }

            return sb.ToString();
        }

        [KernelFunction("GetMyAccountSummary")]
        [Description("Gets a comprehensive summary of the user's account activity.")]
        public async Task<string> GetMyAccountSummaryAsync(
            [Description("The authenticated user's ID")] string userId)
        {
            var carOwnerProfile = await _carOwnerProfileRepository.GetByApplicationUserIdAsync(userId);
            var workshopProfile = await _workShopProfileRepository.GetByApplicationUserIdAsync(userId);

            var sb = new StringBuilder();
            sb.AppendLine("?? Your Account Summary:");
            sb.AppendLine("????????????????????????????");
            sb.AppendLine();

            if (carOwnerProfile != null)
            {
                var carsQuery = await _carRepository.GetAllCarsByCarOwnerProfileIdAsync(carOwnerProfile.Id);
                var cars = carsQuery.ToList();

                var bookings = _bookingRepository.GetAllWithIncludeAsync(b => b.Car)
                    ?.Where(b => b.Car.CarOwnerProfileId == carOwnerProfile.Id)
                    .ToList() ?? [];

                var completedBookings = bookings.Count(b => b.Status == BookingStatus.Completed);
                var pendingBookings = bookings.Count(b => b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed);
                var totalSpent = bookings.Where(b => b.PaidAmount.HasValue).Sum(b => b.PaidAmount!.Value);

                sb.AppendLine("?? As a Car Owner:");
                sb.AppendLine($"   Cars Registered: {cars.Count}");
                sb.AppendLine($"   Total Bookings: {bookings.Count}");
                sb.AppendLine($"   Completed Services: {completedBookings}");
                sb.AppendLine($"   Pending/Active: {pendingBookings}");
                sb.AppendLine($"   Total Spent: {totalSpent:C}");
                sb.AppendLine();
            }

            if (workshopProfile != null)
            {
                var workshopBookings = _bookingRepository.GetBookingsByWorkshopProfileIdAsync(workshopProfile.Id).ToList();
                var completedWorkshopBookings = workshopBookings.Count(b => b.Status == BookingStatus.Completed);
                var pendingWorkshopBookings = workshopBookings.Count(b => b.Status == BookingStatus.Pending);
                var totalEarned = workshopBookings.Where(b => b.PaidAmount.HasValue).Sum(b => b.PaidAmount!.Value);

                sb.AppendLine("?? As a Workshop Owner:");
                sb.AppendLine($"   Workshop: {workshopProfile.Name}");
                sb.AppendLine($"   Rating: ? {workshopProfile.Rating:F1}/5");
                sb.AppendLine($"   Total Bookings: {workshopBookings.Count}");
                sb.AppendLine($"   Completed: {completedWorkshopBookings}");
                sb.AppendLine($"   Pending: {pendingWorkshopBookings}");
                sb.AppendLine($"   Total Earnings: {totalEarned:C}");
                sb.AppendLine();
            }

            if (carOwnerProfile == null && workshopProfile == null)
            {
                sb.AppendLine("?? No profile found. Create a profile to start!");
            }

            return sb.ToString();
        }

        [KernelFunction("GetMyLocation")]
        [Description("Gets the user's saved location information.")]
        public async Task<string> GetMyLocationAsync(
            [Description("The authenticated user's ID")] string userId)
        {
            var carOwnerProfile = await _carOwnerProfileRepository.GetByApplicationUserIdAsync(userId);
            var workshopProfile = await _workShopProfileRepository.GetByApplicationUserIdAsync(userId);

            var sb = new StringBuilder();
            sb.AppendLine("?? Your Location Information:");
            sb.AppendLine();

            if (carOwnerProfile != null)
            {
                sb.AppendLine("Home Location:");
                sb.AppendLine($"   City: {carOwnerProfile.City}");
                sb.AppendLine($"   Governorate: {carOwnerProfile.Governorate}");
                sb.AppendLine($"   Country: {carOwnerProfile.Country}");
                sb.AppendLine();
            }

            if (workshopProfile != null)
            {
                sb.AppendLine("Workshop Location:");
                sb.AppendLine($"   City: {workshopProfile.City}");
                sb.AppendLine($"   Governorate: {workshopProfile.Governorate}");
                sb.AppendLine($"   Country: {workshopProfile.Country}");
                sb.AppendLine($"   Coordinates: {workshopProfile.Latitude}, {workshopProfile.Longitude}");
                sb.AppendLine();
            }

            if (carOwnerProfile == null && workshopProfile == null)
            {
                return "No location information found. Please create a profile first.";
            }

            return sb.ToString();
        }
    }
}
