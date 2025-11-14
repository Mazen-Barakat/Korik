using FluentValidation;
using System;

namespace Korik.Application
{
    public class UpdateCarExpenseDTOValidator : AbstractValidator<UpdateCarExpenseDTO>
    {
        private readonly ICarService _carService;
        private readonly ICarExpenseService _carExpenseService;

        public UpdateCarExpenseDTOValidator(ICarService carService, ICarExpenseService carExpenseService)
        {
            _carExpenseService = carExpenseService; 
            _carService = carService;

            // Id
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than 0.")
                .MustAsync(async (id, cancellation) =>
                {
                    var result = await _carExpenseService.GetByIdAsync(id);
                    return result.Success && result.Data != null;
                }).WithMessage("The specified car expense does not exist.");

            // Amount
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.");

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
