using Korik.Application;
using Korik.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text;

namespace Korik.Infrastructure
{
    /// <summary>
    /// Plugin for handling review-related queries via AI assistant.
    /// </summary>
    public class ReviewPlugin
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ICarOwnerProfileRepository _carOwnerProfileRepository;
        private readonly IWorkShopProfileRepository _workShopProfileRepository;
        private readonly IBookingRepository _bookingRepository;

        public ReviewPlugin(
            IReviewRepository reviewRepository,
            ICarOwnerProfileRepository carOwnerProfileRepository,
            IWorkShopProfileRepository workShopProfileRepository,
            IBookingRepository bookingRepository)
        {
            _reviewRepository = reviewRepository;
            _carOwnerProfileRepository = carOwnerProfileRepository;
            _workShopProfileRepository = workShopProfileRepository;
            _bookingRepository = bookingRepository;
        }

        [KernelFunction("GetMyGivenReviews")]
        [Description("Gets all reviews that the user has given to workshops.")]
        public async Task<string> GetMyGivenReviewsAsync(
            [Description("The authenticated user's ID")] string userId)
        {
            var carOwnerProfile = await _carOwnerProfileRepository.GetByApplicationUserIdWithIncludeAsync(
                userId,
                c => c.Reviews);

            if (carOwnerProfile == null)
            {
                return "You don't have a car owner profile yet.";
            }

            var reviews = await _reviewRepository.GetAllWithIncludeAsync(
                    r => r.WorkShopProfile,
                    r => r.Booking)
                ?.Where(r => r.CarOwnerProfileId == carOwnerProfile.Id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync() ?? [];

            if (!reviews.Any())
            {
                return "You haven't written any reviews yet.";
            }

            var sb = new StringBuilder();
            var avgRating = reviews.Average(r => r.Rating);
            sb.AppendLine($"?? Your Reviews ({reviews.Count} total, Avg: ? {avgRating:F1}/5):");
            sb.AppendLine();

            foreach (var review in reviews.Take(10))
            {
                var stars = new string('?', review.Rating);
                sb.AppendLine($"{stars} ({review.Rating}/5) - {review.WorkShopProfile?.Name ?? "Workshop"}");

                if (!string.IsNullOrEmpty(review.Comment))
                {
                    sb.AppendLine($"   \"{review.Comment}\"");
                }

                sb.AppendLine($"   Date: {review.CreatedAt:yyyy-MM-dd}");

                if (review.PaidAmount.HasValue)
                {
                    sb.AppendLine($"   Cost: {review.PaidAmount:C}");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        [KernelFunction("GetMyReceivedReviews")]
        [Description("Gets all reviews that the workshop has received. For workshop owners.")]
        public async Task<string> GetMyReceivedReviewsAsync(
            [Description("The authenticated user's ID")] string userId)
        {
            var workshopProfile = await _workShopProfileRepository.GetByApplicationUserIdAsync(userId);

            if (workshopProfile == null)
            {
                return "You don't have a workshop profile. This is for workshop owners.";
            }

            var reviews = await _reviewRepository.GetAllReviewsByWorkShopProfileIdAsync(workshopProfile.Id)
                .Include(r => r.CarOwnerProfile)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            if (!reviews.Any())
            {
                return "Your workshop hasn't received any reviews yet.";
            }

            var avgRating = await _reviewRepository.GetAverageRatingsByWorkShopProfileIdAsync(workshopProfile.Id);

            var sb = new StringBuilder();
            sb.AppendLine($"?? Your Workshop Reviews:");
            sb.AppendLine($"Total: {reviews.Count} | Average: ? {avgRating:F1}/5");
            sb.AppendLine();

            foreach (var review in reviews.Take(10))
            {
                var stars = new string('?', review.Rating);
                var customerName = review.CarOwnerProfile != null
                    ? $"{review.CarOwnerProfile.FirstName} {review.CarOwnerProfile.LastName[0]}."
                    : "Customer";

                sb.AppendLine($"{stars} ({review.Rating}/5) by {customerName}");

                if (!string.IsNullOrEmpty(review.Comment))
                {
                    sb.AppendLine($"   \"{review.Comment}\"");
                }

                sb.AppendLine($"   Date: {review.CreatedAt:yyyy-MM-dd}");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        [KernelFunction("GetPendingReviews")]
        [Description("Gets completed bookings that haven't been reviewed yet.")]
        public async Task<string> GetPendingReviewsAsync(
            [Description("The authenticated user's ID")] string userId)
        {
            var carOwnerProfile = await _carOwnerProfileRepository.GetByApplicationUserIdAsync(userId);

            if (carOwnerProfile == null)
            {
                return "You don't have a car owner profile yet.";
            }

            var pendingReviews = await _bookingRepository.GetAllWithIncludeAsync(
                    b => b.Car,
                    b => b.WorkShopProfile,
                    b => b.WorkshopService,
                    b => b.Review)
                ?.Where(b => b.Car.CarOwnerProfileId == carOwnerProfile.Id
                    && b.Status == BookingStatus.Completed
                    && b.Review == null)
                .OrderByDescending(b => b.AppointmentDate)
                .Take(10)
                .ToListAsync() ?? [];

            if (!pendingReviews.Any())
            {
                return "? You've reviewed all your completed bookings!";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"?? {pendingReviews.Count} booking(s) waiting for your review:");
            sb.AppendLine();

            foreach (var booking in pendingReviews)
            {
                sb.AppendLine($"• Booking #{booking.Id}: {booking.WorkshopService?.Service?.Name ?? "Service"}");
                sb.AppendLine($"  Workshop: {booking.WorkShopProfile?.Name ?? "Workshop"}");
                sb.AppendLine($"  Completed: {booking.AppointmentDate:yyyy-MM-dd}");
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
