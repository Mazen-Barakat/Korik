using Korik.Application;
using Korik.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text;

namespace Korik.Infrastructure
{
    public class ExpensePlugin
    {
        private readonly ICarExpenseRepository _carExpenseRepository;
        private readonly ICarRepository _carRepository;
        private readonly ICarOwnerProfileRepository _carOwnerProfileRepository;

        public ExpensePlugin(
            ICarExpenseRepository carExpenseRepository,
            ICarRepository carRepository,
            ICarOwnerProfileRepository carOwnerProfileRepository)
        {
            _carExpenseRepository = carExpenseRepository;
            _carRepository = carRepository;
            _carOwnerProfileRepository = carOwnerProfileRepository;
        }

        [KernelFunction("GetCarExpenses")]
        [Description("Gets all expenses for a specific car. Use this when user asks about expenses, costs, or spending for a particular car.")]
        public async Task<string> GetCarExpensesAsync(
            [Description("The car ID to get expenses for")] int carId,
            [Description("The authenticated user's ID")] string userId)
        {
            var carOwnerProfile = await _carOwnerProfileRepository.GetByApplicationUserIdAsync(userId);

            if (carOwnerProfile == null)
            {
                return "You don't have a car owner profile.";
            }

            var car = await _carRepository.GetByIdAsync(carId);

            if (car == null)
            {
                return $"Car with ID {carId} was not found.";
            }

            if (car.CarOwnerProfileId != carOwnerProfile.Id)
            {
                return "You don't have access to this car's expenses.";
            }

            var expensesQuery = await _carExpenseRepository.GetAllCarExpensesByCarId(carId);
            var expenses = await expensesQuery
                .OrderByDescending(e => e.ExpenseDate)
                .Take(20)
                .ToListAsync();

            if (!expenses.Any())
            {
                return $"No expenses recorded for your {car.Make} {car.Model}. Start tracking to monitor your car costs!";
            }

            var totalExpenses = expenses.Sum(e => e.Amount);

            var sb = new StringBuilder();
            sb.AppendLine($"?? Expenses for {car.Make} {car.Model} ({car.LicensePlate}):");
            sb.AppendLine($"Total (shown): {totalExpenses:C}");
            sb.AppendLine();

            foreach (var expense in expenses)
            {
                var typeIcon = expense.ExpenseType switch
                {
                    ExpenseType.Fuel => "?",
                    ExpenseType.Maintenance => "??",
                    ExpenseType.Repair => "???",
                    ExpenseType.Insurance => "??",
                    _ => "??"
                };

                sb.AppendLine($"{typeIcon} {expense.ExpenseType}: {expense.Amount:C}");
                sb.AppendLine($"   Date: {expense.ExpenseDate:yyyy-MM-dd}");
                if (!string.IsNullOrEmpty(expense.Description))
                {
                    sb.AppendLine($"   Note: {expense.Description}");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        [KernelFunction("GetExpenseSummary")]
        [Description("Gets a summary of expenses across all user's cars. Use this when user asks about total spending, expense overview, or cost summary.")]
        public async Task<string> GetExpenseSummaryAsync(
            [Description("The authenticated user's ID")] string userId,
            [Description("Number of months to look back (default 12)")] int months = 12)
        {
            var carOwnerProfile = await _carOwnerProfileRepository.GetByApplicationUserIdAsync(userId);

            if (carOwnerProfile == null)
            {
                return "You don't have a car owner profile.";
            }

            var carsQuery = await _carRepository.GetAllCarsByCarOwnerProfileIdAsync(carOwnerProfile.Id);
            var cars = await carsQuery.Include(c => c.CarExpenses).ToListAsync();

            if (!cars.Any())
            {
                return "You don't have any cars registered.";
            }

            var startDate = DateTime.UtcNow.AddMonths(-months);
            var allExpenses = cars
                .SelectMany(c => c.CarExpenses ?? Enumerable.Empty<CarExpenses>())
                .Where(e => e.ExpenseDate >= startDate)
                .ToList();

            if (!allExpenses.Any())
            {
                return $"No expenses recorded in the last {months} months.";
            }

            var totalExpenses = allExpenses.Sum(e => e.Amount);
            var byType = allExpenses
                .GroupBy(e => e.ExpenseType)
                .Select(g => new { Type = g.Key, Total = g.Sum(e => e.Amount) })
                .OrderByDescending(x => x.Total)
                .ToList();

            var byCar = cars
                .Select(c => new
                {
                    Car = c,
                    Total = c.CarExpenses?.Where(e => e.ExpenseDate >= startDate).Sum(e => e.Amount) ?? 0
                })
                .Where(x => x.Total > 0)
                .OrderByDescending(x => x.Total)
                .ToList();

            var sb = new StringBuilder();
            sb.AppendLine($"?? Expense Summary (Last {months} Months):");
            sb.AppendLine($"???????????????????????????????????");
            sb.AppendLine($"?? Total Spending: {totalExpenses:C}");
            sb.AppendLine($"?? Average per Month: {totalExpenses / months:C}");
            sb.AppendLine();

            sb.AppendLine("?? By Category:");
            foreach (var item in byType)
            {
                var percentage = (item.Total / totalExpenses) * 100;
                var typeIcon = item.Type switch
                {
                    ExpenseType.Fuel => "?",
                    ExpenseType.Maintenance => "??",
                    ExpenseType.Repair => "???",
                    ExpenseType.Insurance => "??",
                    _ => "??"
                };
                sb.AppendLine($"   {typeIcon} {item.Type}: {item.Total:C} ({percentage:F1}%)");
            }

            sb.AppendLine();
            sb.AppendLine("?? By Car:");
            foreach (var item in byCar)
            {
                sb.AppendLine($"   {item.Car.Make} {item.Car.Model}: {item.Total:C}");
            }

            return sb.ToString();
        }

        [KernelFunction("GetMonthlyExpenses")]
        [Description("Gets monthly expense breakdown for a car. Use this when user asks about monthly spending or expense trends.")]
        public async Task<string> GetMonthlyExpensesAsync(
            [Description("The car ID (optional, if not provided shows all cars)")] int? carId,
            [Description("The authenticated user's ID")] string userId,
            [Description("Number of months to show (default 6)")] int months = 6)
        {
            var carOwnerProfile = await _carOwnerProfileRepository.GetByApplicationUserIdAsync(userId);

            if (carOwnerProfile == null)
            {
                return "You don't have a car owner profile.";
            }

            List<CarExpenses> expenses;
            string carInfo;

            if (carId.HasValue)
            {
                var car = await _carRepository.GetByIdAsync(carId.Value);
                if (car == null || car.CarOwnerProfileId != carOwnerProfile.Id)
                {
                    return "Car not found or you don't have access.";
                }

                var expensesQuery = await _carExpenseRepository.GetAllCarExpensesByCarId(carId.Value);
                expenses = await expensesQuery.ToListAsync();
                carInfo = $"{car.Make} {car.Model}";
            }
            else
            {
                var carsQuery = await _carRepository.GetAllCarsByCarOwnerProfileIdAsync(carOwnerProfile.Id);
                var cars = await carsQuery.Include(c => c.CarExpenses).ToListAsync();
                expenses = cars.SelectMany(c => c.CarExpenses ?? Enumerable.Empty<CarExpenses>()).ToList();
                carInfo = "All Cars";
            }

            var startDate = DateTime.UtcNow.AddMonths(-months + 1);
            var monthlyData = expenses
                .Where(e => e.ExpenseDate >= startDate)
                .GroupBy(e => new { e.ExpenseDate.Year, e.ExpenseDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Total = g.Sum(e => e.Amount),
                    Count = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            if (!monthlyData.Any())
            {
                return $"No expenses recorded in the last {months} months.";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"?? Monthly Expenses - {carInfo}:");
            sb.AppendLine();

            foreach (var month in monthlyData)
            {
                var monthName = new DateTime(month.Year, month.Month, 1).ToString("MMM yyyy");
                sb.AppendLine($"?? {monthName}: {month.Total:C} ({month.Count} transaction(s))");
            }

            var avgMonthly = monthlyData.Average(m => m.Total);
            var maxMonth = monthlyData.OrderByDescending(m => m.Total).First();
            var minMonth = monthlyData.OrderBy(m => m.Total).First();

            sb.AppendLine();
            sb.AppendLine($"?? Statistics:");
            sb.AppendLine($"   Average: {avgMonthly:C}/month");
            sb.AppendLine($"   Highest: {new DateTime(maxMonth.Year, maxMonth.Month, 1):MMM yyyy} ({maxMonth.Total:C})");
            sb.AppendLine($"   Lowest: {new DateTime(minMonth.Year, minMonth.Month, 1):MMM yyyy} ({minMonth.Total:C})");

            return sb.ToString();
        }

        [KernelFunction("GetFuelExpenses")]
        [Description("Gets fuel expenses and calculates fuel efficiency. Use this when user asks about fuel costs, gas spending, or fuel efficiency.")]
        public async Task<string> GetFuelExpensesAsync(
            [Description("The car ID")] int carId,
            [Description("The authenticated user's ID")] string userId)
        {
            var carOwnerProfile = await _carOwnerProfileRepository.GetByApplicationUserIdAsync(userId);

            if (carOwnerProfile == null)
            {
                return "You don't have a car owner profile.";
            }

            var car = await _carRepository.GetByIdAsync(carId);

            if (car == null || car.CarOwnerProfileId != carOwnerProfile.Id)
            {
                return "Car not found or you don't have access.";
            }

            var expensesQuery = await _carExpenseRepository.GetAllCarExpensesByCarId(carId);
            var fuelExpenses = await expensesQuery
                .Where(e => e.ExpenseType == ExpenseType.Fuel)
                .OrderByDescending(e => e.ExpenseDate)
                .ToListAsync();

            if (!fuelExpenses.Any())
            {
                return $"No fuel expenses recorded for your {car.Make} {car.Model}. Track your fuel purchases to monitor efficiency!";
            }

            var totalFuel = fuelExpenses.Sum(e => e.Amount);
            var fuelCount = fuelExpenses.Count;
            var avgFuelUp = totalFuel / fuelCount;

            // Get expenses from last 3 months for recent average
            var threeMonthsAgo = DateTime.UtcNow.AddMonths(-3);
            var recentFuel = fuelExpenses.Where(e => e.ExpenseDate >= threeMonthsAgo).ToList();
            var recentTotal = recentFuel.Sum(e => e.Amount);
            var recentMonthlyAvg = recentFuel.Any() ? recentTotal / 3 : 0;

            var sb = new StringBuilder();
            sb.AppendLine($"? Fuel Expenses - {car.Make} {car.Model}:");
            sb.AppendLine();
            sb.AppendLine($"?? Overall Statistics:");
            sb.AppendLine($"   Total Fuel Spending: {totalFuel:C}");
            sb.AppendLine($"   Number of Fill-ups: {fuelCount}");
            sb.AppendLine($"   Average per Fill-up: {avgFuelUp:C}");
            sb.AppendLine();

            if (recentFuel.Any())
            {
                sb.AppendLine($"?? Last 3 Months:");
                sb.AppendLine($"   Spending: {recentTotal:C}");
                sb.AppendLine($"   Monthly Average: {recentMonthlyAvg:C}");
                sb.AppendLine();
            }

            sb.AppendLine("?? Recent Fill-ups:");
            foreach (var expense in fuelExpenses.Take(5))
            {
                sb.AppendLine($"   {expense.ExpenseDate:yyyy-MM-dd}: {expense.Amount:C}");
                if (!string.IsNullOrEmpty(expense.Description))
                {
                    sb.AppendLine($"      Note: {expense.Description}");
                }
            }

            return sb.ToString();
        }

        [KernelFunction("CompareCarExpenses")]
        [Description("Compares expenses between multiple cars. Use this when user asks to compare costs between their cars.")]
        public async Task<string> CompareCarExpensesAsync(
            [Description("The authenticated user's ID")] string userId)
        {
            var carOwnerProfile = await _carOwnerProfileRepository.GetByApplicationUserIdAsync(userId);

            if (carOwnerProfile == null)
            {
                return "You don't have a car owner profile.";
            }

            var carsQuery = await _carRepository.GetAllCarsByCarOwnerProfileIdAsync(carOwnerProfile.Id);
            var cars = await carsQuery.Include(c => c.CarExpenses).ToListAsync();

            if (cars.Count < 2)
            {
                return "You need at least 2 cars to compare expenses.";
            }

            var oneYearAgo = DateTime.UtcNow.AddYears(-1);

            var sb = new StringBuilder();
            sb.AppendLine("?? Car Expense Comparison (Last 12 Months):");
            sb.AppendLine("???????????????????????????????????????????");
            sb.AppendLine();

            var comparisons = cars.Select(car =>
            {
                var yearExpenses = car.CarExpenses?
                    .Where(e => e.ExpenseDate >= oneYearAgo)
                    .ToList() ?? [];

                return new
                {
                    Car = car,
                    Total = yearExpenses.Sum(e => e.Amount),
                    Fuel = yearExpenses.Where(e => e.ExpenseType == ExpenseType.Fuel).Sum(e => e.Amount),
                    Maintenance = yearExpenses.Where(e => e.ExpenseType == ExpenseType.Maintenance).Sum(e => e.Amount),
                    Repair = yearExpenses.Where(e => e.ExpenseType == ExpenseType.Repair).Sum(e => e.Amount),
                    Insurance = yearExpenses.Where(e => e.ExpenseType == ExpenseType.Insurance).Sum(e => e.Amount)
                };
            }).OrderByDescending(x => x.Total).ToList();

            foreach (var item in comparisons)
            {
                sb.AppendLine($"?? {item.Car.Make} {item.Car.Model} ({item.Car.Year})");
                sb.AppendLine($"   Total: {item.Total:C}");
                sb.AppendLine($"   ? Fuel: {item.Fuel:C}");
                sb.AppendLine($"   ?? Maintenance: {item.Maintenance:C}");
                sb.AppendLine($"   ??? Repairs: {item.Repair:C}");
                sb.AppendLine($"   ?? Insurance: {item.Insurance:C}");
                sb.AppendLine();
            }

            var cheapestCar = comparisons.OrderBy(x => x.Total).First();
            var mostExpensiveCar = comparisons.OrderByDescending(x => x.Total).First();

            sb.AppendLine("?? Insights:");
            sb.AppendLine($"   Most Economical: {cheapestCar.Car.Make} {cheapestCar.Car.Model} ({cheapestCar.Total:C})");
            sb.AppendLine($"   Highest Cost: {mostExpensiveCar.Car.Make} {mostExpensiveCar.Car.Model} ({mostExpensiveCar.Total:C})");

            return sb.ToString();
        }
    }
}
