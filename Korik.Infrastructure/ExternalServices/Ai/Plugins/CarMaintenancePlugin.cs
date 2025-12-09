using Korik.Application;
using Korik.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text;

namespace Korik.Infrastructure
{
    public class CarMaintenancePlugin
    {
        private readonly ICarIndicatorRepository _carIndicatorRepository;
        private readonly ICarRepository _carRepository;
        private readonly ICarOwnerProfileRepository _carOwnerProfileRepository;

        public CarMaintenancePlugin(
            ICarIndicatorRepository carIndicatorRepository,
            ICarRepository carRepository,
            ICarOwnerProfileRepository carOwnerProfileRepository)
        {
            _carIndicatorRepository = carIndicatorRepository;
            _carRepository = carRepository;
            _carOwnerProfileRepository = carOwnerProfileRepository;
        }

        [KernelFunction("GetMaintenanceStatus")]
        [Description("Gets the overall maintenance status for all user's cars. Use this when user asks about maintenance, car health, or what needs attention.")]
        public async Task<string> GetMaintenanceStatusAsync(
            [Description("The authenticated user's ID")] string userId)
        {
            var carOwnerProfile = await _carOwnerProfileRepository.GetByApplicationUserIdAsync(userId);

            if (carOwnerProfile == null)
            {
                return "You don't have a car owner profile yet.";
            }

            var carsQuery = await _carRepository.GetAllCarsByCarOwnerProfileIdAsync(carOwnerProfile.Id);
            var cars = await carsQuery.Include(c => c.CarIndicators).ToListAsync();

            if (cars.Count == 0)
            {
                return "You don't have any cars registered to check maintenance for.";
            }

            var sb = new StringBuilder();
            sb.AppendLine("?? Maintenance Status Overview:");
            sb.AppendLine();

            foreach (var car in cars)
            {
                sb.AppendLine($"?? {car.Make} {car.Model} ({car.LicensePlate}):");

                if (car.CarIndicators == null || !car.CarIndicators.Any())
                {
                    sb.AppendLine("   No maintenance indicators set up yet.");
                    sb.AppendLine();
                    continue;
                }

                var criticalItems = car.CarIndicators.Where(i => i.CarStatus == CarStatus.Critical).ToList();
                var warningItems = car.CarIndicators.Where(i => i.CarStatus == CarStatus.Warning).ToList();
                var normalItems = car.CarIndicators.Where(i => i.CarStatus == CarStatus.Normal).ToList();

                if (criticalItems.Any())
                {
                    sb.AppendLine("   ?? CRITICAL (Immediate attention needed):");
                    foreach (var item in criticalItems)
                    {
                        sb.AppendLine($"      - {FormatIndicatorType(item.IndicatorType)}");
                        sb.AppendLine($"        Next check: {item.NextCheckedDate:yyyy-MM-dd} or at {item.NextMileage:N0} km");
                    }
                }

                if (warningItems.Any())
                {
                    sb.AppendLine("   ?? WARNING (Schedule soon):");
                    foreach (var item in warningItems)
                    {
                        sb.AppendLine($"      - {FormatIndicatorType(item.IndicatorType)}");
                        sb.AppendLine($"        Next check: {item.NextCheckedDate:yyyy-MM-dd} or at {item.NextMileage:N0} km");
                    }
                }

                if (normalItems.Any())
                {
                    sb.AppendLine($"   ?? OK: {string.Join(", ", normalItems.Select(i => FormatIndicatorType(i.IndicatorType)))}");
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        [KernelFunction("GetCarMaintenanceDetails")]
        [Description("Gets detailed maintenance indicators for a specific car. Use this when user asks about maintenance for a particular car.")]
        public async Task<string> GetCarMaintenanceDetailsAsync(
            [Description("The car ID to check maintenance for")] int carId,
            [Description("The authenticated user's ID")] string userId)
        {
            var carOwnerProfile = await _carOwnerProfileRepository.GetByApplicationUserIdAsync(userId);

            if (carOwnerProfile == null)
            {
                return "You don't have a car owner profile.";
            }

            var car = await _carRepository.GetByIdWithIncludeAsync(carId, c => c.CarIndicators);

            if (car == null)
            {
                return $"Car with ID {carId} was not found.";
            }

            if (car.CarOwnerProfileId != carOwnerProfile.Id)
            {
                return "You don't have access to this car.";
            }

            var indicatorsQuery = await _carIndicatorRepository.GetAllCarIndicatorsByCarId(carId);
            var indicators = await indicatorsQuery.ToListAsync();

            if (!indicators.Any())
            {
                return $"No maintenance indicators are set up for your {car.Make} {car.Model}. Consider adding maintenance reminders for oil changes, tire rotation, etc.";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"?? Maintenance Details for {car.Make} {car.Model} ({car.LicensePlate}):");
            sb.AppendLine($"Current Mileage: {car.CurrentMileage:N0} km");
            sb.AppendLine();

            foreach (var indicator in indicators.OrderBy(i => i.CarStatus))
            {
                var statusIcon = indicator.CarStatus switch
                {
                    CarStatus.Critical => "??",
                    CarStatus.Warning => "??",
                    CarStatus.Normal => "??",
                    _ => "?"
                };

                sb.AppendLine($"{statusIcon} {FormatIndicatorType(indicator.IndicatorType)}");
                sb.AppendLine($"   Status: {indicator.CarStatus}");
                sb.AppendLine($"   Last Checked: {indicator.LastCheckedDate:yyyy-MM-dd}");
                sb.AppendLine($"   Next Check Date: {indicator.NextCheckedDate:yyyy-MM-dd}");
                
                if (indicator.NextMileage > 0)
                {
                    var mileageRemaining = indicator.NextMileage - car.CurrentMileage;
                    sb.AppendLine($"   Next Check Mileage: {indicator.NextMileage:N0} km ({mileageRemaining:N0} km remaining)");
                }

                if (indicator.TimeDifferenceAsPercentage > 0)
                {
                    sb.AppendLine($"   Time Progress: {indicator.TimeDifferenceAsPercentage:P0}");
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        [KernelFunction("GetUpcomingMaintenance")]
        [Description("Gets maintenance items that are due soon across all cars. Use this when user asks what maintenance is coming up or due soon.")]
        public async Task<string> GetUpcomingMaintenanceAsync(
            [Description("The authenticated user's ID")] string userId,
            [Description("Number of days to look ahead (default 30)")] int daysAhead = 30)
        {
            var carOwnerProfile = await _carOwnerProfileRepository.GetByApplicationUserIdAsync(userId);

            if (carOwnerProfile == null)
            {
                return "You don't have a car owner profile yet.";
            }

            var carsQuery = await _carRepository.GetAllCarsByCarOwnerProfileIdAsync(carOwnerProfile.Id);
            var cars = await carsQuery.Include(c => c.CarIndicators).ToListAsync();

            if (cars.Count == 0)
            {
                return "You don't have any cars registered.";
            }

            var upcomingDate = DateTime.UtcNow.AddDays(daysAhead);
            var upcomingItems = new List<(Car car, CarIndicator indicator)>();

            foreach (var car in cars)
            {
                if (car.CarIndicators == null) continue;

                foreach (var indicator in car.CarIndicators)
                {
                    if (indicator.NextCheckedDate <= upcomingDate || indicator.CarStatus != CarStatus.Normal)
                    {
                        upcomingItems.Add((car, indicator));
                    }
                }
            }

            if (!upcomingItems.Any())
            {
                return $"No maintenance items are due in the next {daysAhead} days. All your cars are in good shape!";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"?? Maintenance Due in the Next {daysAhead} Days:");
            sb.AppendLine();

            var orderedItems = upcomingItems
                .OrderBy(x => x.indicator.CarStatus)
                .ThenBy(x => x.indicator.NextCheckedDate);

            foreach (var (car, indicator) in orderedItems)
            {
                var statusIcon = indicator.CarStatus switch
                {
                    CarStatus.Critical => "?? URGENT",
                    CarStatus.Warning => "?? Soon",
                    _ => "?? Upcoming"
                };

                var daysUntil = (indicator.NextCheckedDate - DateTime.UtcNow).Days;

                sb.AppendLine($"{statusIcon}: {FormatIndicatorType(indicator.IndicatorType)}");
                sb.AppendLine($"   Car: {car.Make} {car.Model} ({car.LicensePlate})");
                sb.AppendLine($"   Due: {indicator.NextCheckedDate:yyyy-MM-dd} ({(daysUntil <= 0 ? "OVERDUE" : $"in {daysUntil} days")})");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        [KernelFunction("GetMaintenanceRecommendations")]
        [Description("Gets personalized maintenance recommendations based on car status. Use this when user asks for maintenance advice or recommendations.")]
        public async Task<string> GetMaintenanceRecommendationsAsync(
            [Description("The authenticated user's ID")] string userId)
        {
            var carOwnerProfile = await _carOwnerProfileRepository.GetByApplicationUserIdAsync(userId);

            if (carOwnerProfile == null)
            {
                return "You don't have a car owner profile yet.";
            }

            var carsQuery = await _carRepository.GetAllCarsByCarOwnerProfileIdAsync(carOwnerProfile.Id);
            var cars = await carsQuery.Include(c => c.CarIndicators).ToListAsync();

            if (cars.Count == 0)
            {
                return "You don't have any cars registered. Add a car to get personalized maintenance recommendations.";
            }

            var sb = new StringBuilder();
            sb.AppendLine("?? Personalized Maintenance Recommendations:");
            sb.AppendLine();

            foreach (var car in cars)
            {
                sb.AppendLine($"?? {car.Make} {car.Model} ({car.Year}) - {car.LicensePlate}:");

                // General recommendations based on car data
                if (car.CurrentMileage > 100000)
                {
                    sb.AppendLine("   • High mileage vehicle - consider more frequent oil changes and inspections");
                }

                if (car.Year < DateTime.Now.Year - 10)
                {
                    sb.AppendLine("   • Older vehicle - pay extra attention to belts, hoses, and suspension components");
                }

                if (car.CarIndicators == null || !car.CarIndicators.Any())
                {
                    sb.AppendLine("   • Set up maintenance indicators to track:");
                    sb.AppendLine("     - Oil Change (every 5,000-10,000 km)");
                    sb.AppendLine("     - Tire Rotation (every 10,000 km)");
                    sb.AppendLine("     - AC Service (annually)");
                    sb.AppendLine("     - Battery Check (every 2-3 years)");
                    sb.AppendLine("     - General Maintenance inspection");
                }
                else
                {
                    var criticalItems = car.CarIndicators.Where(i => i.CarStatus == CarStatus.Critical).ToList();
                    var warningItems = car.CarIndicators.Where(i => i.CarStatus == CarStatus.Warning).ToList();

                    if (criticalItems.Any())
                    {
                        sb.AppendLine("   ?? PRIORITY - Address these immediately:");
                        foreach (var item in criticalItems)
                        {
                            sb.AppendLine($"      - {FormatIndicatorType(item.IndicatorType)}");
                        }
                    }

                    if (warningItems.Any())
                    {
                        sb.AppendLine("   ?? Schedule these soon:");
                        foreach (var item in warningItems)
                        {
                            sb.AppendLine($"      - {FormatIndicatorType(item.IndicatorType)}");
                        }
                    }

                    if (!criticalItems.Any() && !warningItems.Any())
                    {
                        sb.AppendLine("   ? All tracked maintenance items are up to date!");
                    }
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static string FormatIndicatorType(IndicatorType type)
        {
            return type switch
            {
                IndicatorType.ACService => "AC Service",
                IndicatorType.CarLicenseAndEnsuranceExpiry => "License & Insurance Expiry",
                IndicatorType.GeneralMaintenance => "General Maintenance",
                IndicatorType.OilChange => "Oil Change",
                IndicatorType.BatteryHealth => "Battery Health",
                IndicatorType.TireChange => "Tire Change/Rotation",
                _ => type.ToString()
            };
        }
    }
}
