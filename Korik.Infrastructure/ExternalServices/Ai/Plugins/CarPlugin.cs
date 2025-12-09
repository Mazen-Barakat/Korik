using Korik.Application;
using Korik.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text;

namespace Korik.Infrastructure
{
    public class CarPlugin
    {
        private readonly ICarRepository _carRepository;
        private readonly ICarOwnerProfileRepository _carOwnerProfileRepository;

        public CarPlugin(
            ICarRepository carRepository,
            ICarOwnerProfileRepository carOwnerProfileRepository)
        {
            _carRepository = carRepository;
            _carOwnerProfileRepository = carOwnerProfileRepository;
        }

        [KernelFunction("GetMyCars")]
        [Description("Gets all cars owned by the current user. Use this when user asks about their cars, vehicles, or car list.")]
        public async Task<string> GetMyCarsAsync(
            [Description("The authenticated user's ID")] string userId)
        {
            var carOwnerProfile = await _carOwnerProfileRepository.GetByApplicationUserIdAsync(userId);

            if (carOwnerProfile == null)
            {
                return "You don't have a car owner profile yet. Please create one to add your cars.";
            }

            var carsQuery = await _carRepository.GetAllCarsByCarOwnerProfileIdAsync(carOwnerProfile.Id);
            var cars = await carsQuery.ToListAsync();

            if (cars.Count == 0)
            {
                return "You don't have any cars registered yet. Would you like to add a car?";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"You have {cars.Count} registered car(s):");

            foreach (var car in cars)
            {
                sb.AppendLine($"- {car.Make} {car.Model} ({car.Year})");
                sb.AppendLine($"  License Plate: {car.LicensePlate}");
                sb.AppendLine($"  Mileage: {car.CurrentMileage:N0} km");
                sb.AppendLine($"  Engine: {car.EngineCapacity}cc, {car.FuelType}");
                sb.AppendLine($"  Transmission: {car.TransmissionType}");
                sb.AppendLine($"  Origin: {car.Origin}");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        [KernelFunction("GetCarDetails")]
        [Description("Gets detailed information about a specific car by its ID. Use this when user asks about a particular car's details.")]
        public async Task<string> GetCarDetailsAsync(
            [Description("The car ID to retrieve")] int carId,
            [Description("The authenticated user's ID")] string userId)
        {
            var carOwnerProfile = await _carOwnerProfileRepository.GetByApplicationUserIdAsync(userId);

            if (carOwnerProfile == null)
            {
                return "You don't have a car owner profile.";
            }

            var car = await _carRepository.GetByIdWithIncludeAsync(carId, c => c.CarIndicators, c => c.Bookings);

            if (car == null)
            {
                return $"Car with ID {carId} was not found.";
            }

            if (car.CarOwnerProfileId != carOwnerProfile.Id)
            {
                return "You don't have access to this car.";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"Car Details - {car.Make} {car.Model}:");
            sb.AppendLine($"Year: {car.Year}");
            sb.AppendLine($"License Plate: {car.LicensePlate}");
            sb.AppendLine($"Current Mileage: {car.CurrentMileage:N0} km");
            sb.AppendLine($"Engine Capacity: {car.EngineCapacity}cc");
            sb.AppendLine($"Fuel Type: {car.FuelType}");
            sb.AppendLine($"Transmission: {car.TransmissionType}");
            sb.AppendLine($"Origin: {car.Origin}");
            sb.AppendLine($"Total Bookings: {car.Bookings?.Count ?? 0}");

            if (car.CarIndicators?.Any() == true)
            {
                sb.AppendLine();
                sb.AppendLine("Maintenance Indicators:");
                foreach (var indicator in car.CarIndicators)
                {
                    sb.AppendLine($"  - {indicator.IndicatorType}: {indicator.CarStatus}");
                }
            }

            return sb.ToString();
        }

        [KernelFunction("GetCarByLicensePlate")]
        [Description("Finds a car by its license plate number. Use this when user mentions a license plate to identify a car.")]
        public async Task<string> GetCarByLicensePlateAsync(
            [Description("The license plate number to search for")] string licensePlate,
            [Description("The authenticated user's ID")] string userId)
        {
            var carOwnerProfile = await _carOwnerProfileRepository.GetByApplicationUserIdAsync(userId);

            if (carOwnerProfile == null)
            {
                return "You don't have a car owner profile.";
            }

            var carsQuery = await _carRepository.GetAllCarsByCarOwnerProfileIdAsync(carOwnerProfile.Id);
            var car = await carsQuery
                .FirstOrDefaultAsync(c => c.LicensePlate.ToLower().Contains(licensePlate.ToLower()));

            if (car == null)
            {
                return $"No car found with license plate containing '{licensePlate}'.";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"Found car with license plate {car.LicensePlate}:");
            sb.AppendLine($"Car ID: {car.Id}");
            sb.AppendLine($"Make/Model: {car.Make} {car.Model} ({car.Year})");
            sb.AppendLine($"Current Mileage: {car.CurrentMileage:N0} km");
            sb.AppendLine($"Engine: {car.EngineCapacity}cc {car.FuelType}");
            sb.AppendLine($"Transmission: {car.TransmissionType}");

            return sb.ToString();
        }

        [KernelFunction("GetCarsSummary")]
        [Description("Gets a quick summary of all user's cars with their current status. Use this for a brief overview.")]
        public async Task<string> GetCarsSummaryAsync(
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
                return "You don't have any cars registered.";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"Quick Summary - Your {cars.Count} Car(s):");

            foreach (var car in cars)
            {
                var criticalCount = car.CarIndicators?.Count(i => i.CarStatus == CarStatus.Critical) ?? 0;
                var warningCount = car.CarIndicators?.Count(i => i.CarStatus == CarStatus.Warning) ?? 0;

                var statusIcon = criticalCount > 0 ? "??" : warningCount > 0 ? "??" : "??";

                sb.AppendLine($"{statusIcon} {car.Make} {car.Model} ({car.LicensePlate}) - {car.CurrentMileage:N0} km");

                if (criticalCount > 0)
                {
                    sb.AppendLine($"   ?? {criticalCount} critical maintenance item(s)!");
                }
                if (warningCount > 0)
                {
                    sb.AppendLine($"   ? {warningCount} warning(s)");
                }
            }

            return sb.ToString();
        }
    }
}
