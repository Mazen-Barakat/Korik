using Korik.Application;
using Korik.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text;

namespace Korik.Infrastructure
{
    public class ServicePlugin
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISubcategoryRepository _subcategoryRepository;
        private readonly IWorkshopServiceRepository _workshopServiceRepository;

        public ServicePlugin(
            IServiceRepository serviceRepository,
            ICategoryRepository categoryRepository,
            ISubcategoryRepository subcategoryRepository,
            IWorkshopServiceRepository workshopServiceRepository)
        {
            _serviceRepository = serviceRepository;
            _categoryRepository = categoryRepository;
            _subcategoryRepository = subcategoryRepository;
            _workshopServiceRepository = workshopServiceRepository;
        }

        [KernelFunction("GetAllCategories")]
        [Description("Gets all available service categories. Use this when user asks what services are available or wants to browse categories.")]
        public async Task<string> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllWithIncludeAsync(c => c.Subcategories)
                ?.OrderBy(c => c.DisplayOrder)
                .ToListAsync() ?? [];

            if (!categories.Any())
            {
                return "No service categories are available at the moment.";
            }

            var sb = new StringBuilder();
            sb.AppendLine("?? Available Service Categories:");
            sb.AppendLine();

            foreach (var category in categories)
            {
                sb.AppendLine($"?? {category.Name} (ID: {category.Id})");
                
                if (category.Subcategories?.Any() == true)
                {
                    foreach (var subcategory in category.Subcategories)
                    {
                        sb.AppendLine($"   ?? {subcategory.Name} (ID: {subcategory.Id})");
                    }
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        [KernelFunction("GetServicesInCategory")]
        [Description("Gets all services within a specific category. Use this when user asks about services in a particular category.")]
        public async Task<string> GetServicesInCategoryAsync(
            [Description("The category ID")] int categoryId)
        {
            var category = await _categoryRepository.GetByIdWithIncludeAsync(categoryId, c => c.Subcategories);

            if (category == null)
            {
                return $"Category with ID {categoryId} was not found.";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"?? Services in {category.Name}:");
            sb.AppendLine();

            if (category.Subcategories == null || !category.Subcategories.Any())
            {
                return $"No subcategories found in {category.Name}.";
            }

            foreach (var subcategory in category.Subcategories)
            {
                sb.AppendLine($"?? {subcategory.Name}:");

                var services = await _serviceRepository.GetBySubcategoryIdAsync(subcategory.Id)
                    ?.ToListAsync() ?? [];

                if (services.Any())
                {
                    foreach (var service in services)
                    {
                        sb.AppendLine($"   • {service.Name} (ID: {service.Id})");
                        if (!string.IsNullOrEmpty(service.Description))
                        {
                            sb.AppendLine($"     {service.Description}");
                        }
                    }
                }
                else
                {
                    sb.AppendLine("   No services available");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        [KernelFunction("SearchServices")]
        [Description("Searches for services by name or description. Use this when user asks about a specific type of service like 'oil change' or 'brake repair'.")]
        public async Task<string> SearchServicesAsync(
            [Description("The service name or keyword to search for")] string searchTerm)
        {
            var services = await _serviceRepository.GetAllWithIncludeAsync(s => s.Subcategory, s => s.Subcategory.Category)
                ?.Where(s => s.Name.ToLower().Contains(searchTerm.ToLower()) ||
                            s.Description.ToLower().Contains(searchTerm.ToLower()))
                .Take(10)
                .ToListAsync() ?? [];

            if (!services.Any())
            {
                return $"No services found matching '{searchTerm}'. Try a different search term or browse categories.";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"?? Services matching '{searchTerm}':");
            sb.AppendLine();

            foreach (var service in services)
            {
                sb.AppendLine($"• {service.Name} (ID: {service.Id})");
                sb.AppendLine($"  Category: {service.Subcategory?.Category?.Name ?? "N/A"} > {service.Subcategory?.Name ?? "N/A"}");
                if (!string.IsNullOrEmpty(service.Description))
                {
                    sb.AppendLine($"  {service.Description}");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        [KernelFunction("FindWorkshopsForService")]
        [Description("Finds workshops that offer a specific service. Use this when user wants to find where they can get a specific service done.")]
        public async Task<string> FindWorkshopsForServiceAsync(
            [Description("The service ID to find workshops for")] int serviceId,
            [Description("City to filter by (optional)")] string? city = null,
            [Description("Car origin filter: European, Asian, American (optional)")] string? carOrigin = null)
        {
            var service = await _serviceRepository.GetByIdAsync(serviceId);

            if (service == null)
            {
                return $"Service with ID {serviceId} was not found.";
            }

            CarOrigin? origin = null;
            if (!string.IsNullOrEmpty(carOrigin) && Enum.TryParse<CarOrigin>(carOrigin, true, out var parsedOrigin))
            {
                origin = parsedOrigin;
            }

            var result = await _workshopServiceRepository.SearchWorkshopsAsync(
                serviceId: serviceId,
                origin: origin,
                city: city,
                latitude: null,
                longitude: null,
                appointmentDate: DateTime.UtcNow,
                pageNumber: 1,
                pageSize: 10);

            if (result.Items == null || !result.Items.Any())
            {
                return $"No workshops found offering '{service.Name}'" + (city != null ? $" in {city}" : "") + ".";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"?? Workshops offering '{service.Name}':");
            sb.AppendLine();

            foreach (var workshopService in result.Items)
            {
                var workshop = workshopService.WorkShopProfile;
                var priceRange = workshopService.MinPrice == workshopService.MaxPrice
                    ? $"{workshopService.MinPrice:C}"
                    : $"{workshopService.MinPrice:C} - {workshopService.MaxPrice:C}";

                sb.AppendLine($"?? {workshop?.Name ?? "Unknown Workshop"}");
                sb.AppendLine($"   Price: {priceRange}");
                sb.AppendLine($"   Duration: ~{workshopService.Duration} minutes");
                sb.AppendLine($"   Car Origin: {workshopService.Origin}");
                if (workshop != null)
                {
                    sb.AppendLine($"   Location: {workshop.City}, {workshop.Governorate}");
                    sb.AppendLine($"   Rating: ? {workshop.Rating:F1}/5");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        [KernelFunction("GetServiceDetails")]
        [Description("Gets detailed information about a specific service including pricing from workshops. Use this when user asks about a particular service.")]
        public async Task<string> GetServiceDetailsAsync(
            [Description("The service ID")] int serviceId)
        {
            var service = await _serviceRepository.GetByIdWithIncludeAsync(
                serviceId,
                s => s.Subcategory,
                s => s.WorkshopServices);

            if (service == null)
            {
                return $"Service with ID {serviceId} was not found.";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"?? Service: {service.Name}");
            sb.AppendLine();
            sb.AppendLine($"Description: {service.Description}");
            sb.AppendLine($"Category: {service.Subcategory?.Name ?? "N/A"}");

            if (service.WorkshopServices?.Any() == true)
            {
                var minPrice = service.WorkshopServices.Min(ws => ws.MinPrice);
                var maxPrice = service.WorkshopServices.Max(ws => ws.MaxPrice);
                var avgDuration = service.WorkshopServices.Average(ws => ws.Duration);

                sb.AppendLine();
                sb.AppendLine($"?? Price Range: {minPrice:C} - {maxPrice:C}");
                sb.AppendLine($"?? Average Duration: ~{avgDuration:F0} minutes");
                sb.AppendLine($"?? Available at {service.WorkshopServices.Count} workshop(s)");
            }
            else
            {
                sb.AppendLine();
                sb.AppendLine("No workshops currently offer this service.");
            }

            return sb.ToString();
        }

        [KernelFunction("GetPopularServices")]
        [Description("Gets a list of popular/commonly requested services. Use this when user asks for recommendations or popular services.")]
        public async Task<string> GetPopularServicesAsync()
        {
            // Get services that have the most workshop offerings
            var services = await _serviceRepository.GetAllWithIncludeAsync(
                    s => s.Subcategory,
                    s => s.WorkshopServices)
                ?.OrderByDescending(s => s.WorkshopServices.Count)
                .Take(10)
                .ToListAsync() ?? [];

            if (!services.Any())
            {
                return "No services are available at the moment.";
            }

            var sb = new StringBuilder();
            sb.AppendLine("?? Popular Services:");
            sb.AppendLine();

            int rank = 1;
            foreach (var service in services)
            {
                var workshopCount = service.WorkshopServices?.Count ?? 0;
                
                sb.AppendLine($"{rank}. {service.Name}");
                sb.AppendLine($"   Category: {service.Subcategory?.Name ?? "N/A"}");
                sb.AppendLine($"   Available at {workshopCount} workshop(s)");

                if (service.WorkshopServices?.Any() == true)
                {
                    var minPrice = service.WorkshopServices.Min(ws => ws.MinPrice);
                    var maxPrice = service.WorkshopServices.Max(ws => ws.MaxPrice);
                    sb.AppendLine($"   Price range: {minPrice:C} - {maxPrice:C}");
                }
                
                sb.AppendLine();
                rank++;
            }

            return sb.ToString();
        }
    }
}
