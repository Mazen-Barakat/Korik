using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class CreateCarExpenseDTOValidator : AbstractValidator<CreateCarExpanseDTO>
    {
        private readonly ICarService _carService;

        public CreateCarExpenseDTOValidator(ICarService carService)
        {
            _carService = carService;

            // Amount
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.")
                .Must(amount => amount.ToString("F2").Length <= 10).WithMessage("Amount must have up to 10 digits in total and 2 decimal places.");

            // Description
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            // ExpenseDate
            RuleFor(x => x.ExpenseDate)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("ExpenseDate cannot be in the future.");

            // ExpenseType
            RuleFor(x => x.ExpenseType)
                .IsInEnum().WithMessage("Invalid ExpenseType.");

            // CarId
            RuleFor(x => x.CarId)
                .GreaterThan(0).WithMessage("CarId must be greater than 0.")
                .MustAsync(async (carId, cancellation) =>
                {
                    var result = await _carService.GetByIdAsync(carId);
                    return result.Success && result.Data != null;
                }).WithMessage("The specified CarId does not exist.");
        }
    }
}
