using Korik.Application;
using Korik.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text;

namespace Korik.Infrastructure
{
    public class WorkshopPlugin
    {
        private readonly IWorkShopProfileRepository _workShopProfileRepository;
        private readonly IWorkshopServiceRepository _workshopServiceRepository;
        private readonly IReviewRepository _reviewRepository;

        public WorkshopPlugin(
            IWorkShopProfileRepository workShopProfileRepository,
            IWorkshopServiceRepository workshopServiceRepository,
            IReviewRepository reviewRepository)
        {
            _workShopProfileRepository = workShopProfileRepository;
            _workshopServiceRepository = workshopServiceRepository;
            _reviewRepository = reviewRepository;
        }

        [KernelFunction("SearchWorkshops")]
        [Description("Searches for workshops based on location, type, or name. Use this when user wants to find a workshop or asks for workshop recommendations.")]
        public async Task<string> SearchWorkshopsAsync(
            [Description("City name to search in (optional)")] string? city = null,
            [Description("Workshop name to search for (optional)")] string? name = null,
            [Description("Workshop type: Independent, MaintainanceCenter, Specialized, Mobile (optional)")] string? workshopType = null,
            [Description("Car origin filter: European, Asian, American (optional)")] string? carOrigin = null)
        {
            WorkShopType? type = null;
            if (!string.IsNullOrEmpty(workshopType) && Enum.TryParse<WorkShopType>(workshopType, true, out var parsedType))
            {
                type = parsedType;
            }

            CarOrigin? origin = null;
            if (!string.IsNullOrEmpty(carOrigin) && Enum.TryParse<CarOrigin>(carOrigin, true, out var parsedOrigin))
            {
                origin = parsedOrigin;
            }

            var result = await _workShopProfileRepository.FilterWorkshopsAsync(
                PageNumber: 1,
                PageSize: 10,
                Name: name,
                Latitude: null,
                Longitude: null,
                Country: null,
                Governorate: null,
                City: city,
                DESCRating: true,
                WorkShopType: type,
                Origin: origin);

            if (result.Items == null || !result.Items.Any())
            {
                return "No workshops found matching your criteria. Try broadening your search.";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"Found {result.TotalRecords} workshop(s):");
            sb.AppendLine();

            foreach (var workshop in result.Items)
            {
                var stars = workshop.Rating > 0 ? $"? {workshop.Rating:F1}/5" : "No ratings yet";
                
                sb.AppendLine($"?? {workshop.Name}");
                sb.AppendLine($"   Type: {workshop.WorkShopType}");
                sb.AppendLine($"   Location: {workshop.City}, {workshop.Governorate}, {workshop.Country}");
                sb.AppendLine($"   Rating: {stars}");
                sb.AppendLine($"   Technicians: {workshop.NumbersOfTechnicians}");
                sb.AppendLine($"   Verified: {(workshop.VerificationStatus == VerificationStatus.Verified ? "? Yes" : "? No")}");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        [KernelFunction("GetWorkshopDetails")]
        [Description("Gets detailed information about a specific workshop. Use this when user asks about a particular workshop.")]
        public async Task<string> GetWorkshopDetailsAsync(
            [Description("The workshop profile ID")] int workshopId)
        {
            var workshop = await _workShopProfileRepository.GetByIdWithIncludeAsync(
                workshopId,
                w => w.WorkingHours,
                w => w.WorkshopServices);

            if (workshop == null)
            {
                return $"Workshop with ID {workshopId} was not found.";
            }

            var avgRating = await _reviewRepository.GetAverageRatingsByWorkShopProfileIdAsync(workshopId);

            var sb = new StringBuilder();
            sb.AppendLine($"?? {workshop.Name}");
            sb.AppendLine();
            sb.AppendLine($"Description: {workshop.Description}");
            sb.AppendLine($"Type: {workshop.WorkShopType}");
            sb.AppendLine($"Location: {workshop.City}, {workshop.Governorate}, {workshop.Country}");
            sb.AppendLine($"Phone: {workshop.PhoneNumber}");
            sb.AppendLine($"Rating: ? {avgRating:F1}/5");
            sb.AppendLine($"Technicians: {workshop.NumbersOfTechnicians}");
            sb.AppendLine($"Verified: {(workshop.VerificationStatus == VerificationStatus.Verified ? "? Verified" : "Pending verification")}");

            if (workshop.WorkingHours?.Any() == true)
            {
                sb.AppendLine();
                sb.AppendLine("?? Working Hours:");
                foreach (var hours in workshop.WorkingHours.OrderBy(h => h.Day))
                {
                    if (hours.IsClosed)
                    {
                        sb.AppendLine($"   {hours.Day}: Closed");
                    }
                    else
                    {
                        sb.AppendLine($"   {hours.Day}: {hours.From:HH\\:mm} - {hours.To:HH\\:mm}");
                    }
                }
            }

            if (workshop.WorkshopServices?.Any() == true)
            {
                sb.AppendLine();
                sb.AppendLine($"?? Services Offered: {workshop.WorkshopServices.Count} service(s)");
            }

            return sb.ToString();
        }

        [KernelFunction("GetWorkshopReviews")]
        [Description("Gets reviews for a specific workshop. Use this when user asks about workshop reviews or ratings.")]
        public async Task<string> GetWorkshopReviewsAsync(
            [Description("The workshop profile ID")] int workshopId)
        {
            var workshop = await _workShopProfileRepository.GetByIdAsync(workshopId);

            if (workshop == null)
            {
                return $"Workshop with ID {workshopId} was not found.";
            }

            var reviews = await _reviewRepository.GetAllReviewsByWorkShopProfileIdAsync(workshopId)
                .Include(r => r.CarOwnerProfile)
                .OrderByDescending(r => r.CreatedAt)
                .Take(10)
                .ToListAsync();

            if (!reviews.Any())
            {
                return $"{workshop.Name} doesn't have any reviews yet.";
            }

            var avgRating = await _reviewRepository.GetAverageRatingsByWorkShopProfileIdAsync(workshopId);

            var sb = new StringBuilder();
            sb.AppendLine($"?? Reviews for {workshop.Name}");
            sb.AppendLine($"Average Rating: ? {avgRating:F1}/5 ({reviews.Count} reviews shown)");
            sb.AppendLine();

            foreach (var review in reviews)
            {
                var stars = new string('?', review.Rating);
                sb.AppendLine($"{stars} ({review.Rating}/5)");
                if (!string.IsNullOrEmpty(review.Comment))
                {
                    sb.AppendLine($"\"{review.Comment}\"");
                }
                sb.AppendLine($"Date: {review.CreatedAt:yyyy-MM-dd}");
                if (review.PaidAmount.HasValue)
                {
                    sb.AppendLine($"Service Cost: {review.PaidAmount:C}");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        [KernelFunction("GetNearbyWorkshops")]
        [Description("Gets workshops near a specific location using coordinates. Use this when user asks for nearby workshops or workshops near them.")]
        public async Task<string> GetNearbyWorkshopsAsync(
            [Description("Latitude coordinate")] decimal latitude,
            [Description("Longitude coordinate")] decimal longitude,
            [Description("Maximum number of results (default 5)")] int maxResults = 5)
        {
            var result = await _workShopProfileRepository.FilterWorkshopsAsync(
                PageNumber: 1,
                PageSize: maxResults,
                Name: null,
                Latitude: latitude,
                Longitude: longitude,
                Country: null,
                Governorate: null,
                City: null,
                DESCRating: true,
                WorkShopType: null,
                Origin: null);

            if (result.Items == null || !result.Items.Any())
            {
                return "No workshops found near your location.";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"Found {result.Items.Count()} workshop(s) near you:");
            sb.AppendLine();

            foreach (var workshop in result.Items)
            {
                var stars = workshop.Rating > 0 ? $"? {workshop.Rating:F1}/5" : "No ratings";
                
                sb.AppendLine($"?? {workshop.Name} ({stars})");
                sb.AppendLine($"   {workshop.City}, {workshop.Governorate}");
                sb.AppendLine($"   Type: {workshop.WorkShopType}");
                sb.AppendLine($"   Phone: {workshop.PhoneNumber}");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        [KernelFunction("GetTopRatedWorkshops")]
        [Description("Gets the highest-rated workshops. Use this when user asks for best workshops or top-rated recommendations.")]
        public async Task<string> GetTopRatedWorkshopsAsync(
            [Description("City to filter by (optional)")] string? city = null,
            [Description("Number of results (default 5)")] int count = 5)
        {
            var result = await _workShopProfileRepository.FilterWorkshopsAsync(
                PageNumber: 1,
                PageSize: count,
                Name: null,
                Latitude: null,
                Longitude: null,
                Country: null,
                Governorate: null,
                City: city,
                DESCRating: true,
                WorkShopType: null,
                Origin: null);

            if (result.Items == null || !result.Items.Any())
            {
                return city != null 
                    ? $"No workshops found in {city}." 
                    : "No workshops found.";
            }

            var sb = new StringBuilder();
            var locationText = city != null ? $" in {city}" : "";
            sb.AppendLine($"?? Top Rated Workshops{locationText}:");
            sb.AppendLine();

            int rank = 1;
            foreach (var workshop in result.Items)
            {
                sb.AppendLine($"#{rank} {workshop.Name}");
                sb.AppendLine($"   ? {workshop.Rating:F1}/5");
                sb.AppendLine($"   ?? {workshop.City}, {workshop.Governorate}");
                sb.AppendLine($"   Type: {workshop.WorkShopType}");
                sb.AppendLine();
                rank++;
            }

            return sb.ToString();
        }

        [KernelFunction("GetMyWorkshopProfile")]
        [Description("Gets the workshop profile for a workshop owner. Use this when a workshop owner asks about their own profile or business.")]
        public async Task<string> GetMyWorkshopProfileAsync(
            [Description("The authenticated user's ID")] string userId)
        {
            var workshop = await _workShopProfileRepository.GetByApplicationUserIdWithIncludeAsync(
                userId,
                w => w.WorkingHours,
                w => w.WorkshopServices,
                w => w.Reviews);

            if (workshop == null)
            {
                return "You don't have a workshop profile. This function is for workshop owners.";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"?? Your Workshop Profile: {workshop.Name}");
            sb.AppendLine();
            sb.AppendLine($"Description: {workshop.Description}");
            sb.AppendLine($"Type: {workshop.WorkShopType}");
            sb.AppendLine($"Location: {workshop.City}, {workshop.Governorate}, {workshop.Country}");
            sb.AppendLine($"Phone: {workshop.PhoneNumber}");
            sb.AppendLine($"Rating: ? {workshop.Rating:F1}/5 ({workshop.Reviews?.Count ?? 0} reviews)");
            sb.AppendLine($"Technicians: {workshop.NumbersOfTechnicians}");
            sb.AppendLine($"Verification Status: {workshop.VerificationStatus}");
            sb.AppendLine($"Services Offered: {workshop.WorkshopServices?.Count ?? 0}");
            sb.AppendLine($"Member Since: {workshop.CreatedAt:yyyy-MM-dd}");

            return sb.ToString();
        }
    }
}
