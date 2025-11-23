using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class CreateWorkShopWorkingHoursDTOValidator : AbstractValidator<CreateWorkShopWorkingHoursDTO>
    {
        private readonly IWorkShopWorkingHoursService _workingHoursService;

        public CreateWorkShopWorkingHoursDTOValidator(IWorkShopWorkingHoursService workingHoursService)
        {
            _workingHoursService = workingHoursService;

            RuleFor(x => x.Day)
                .IsInEnum().WithMessage("Day must be a valid day of the week.");

            RuleFor(x => x.From)
                .NotEmpty().WithMessage("From time is required.")
                .Must((model, from) => !model.IsClosed || from == TimeOnly.MinValue)
                .WithMessage("From time should be empty when workshop is closed.");

            RuleFor(x => x.To)
                .NotEmpty().WithMessage("To time is required.")
                .Must((model, to) => !model.IsClosed || to == TimeOnly.MinValue)
                .WithMessage("To time should be empty when workshop is closed.")
                .Must((model, to) => model.IsClosed || to > model.From)
                .WithMessage("To time must be after From time.");

            RuleFor(x => x.WorkShopProfileId)
                .GreaterThan(0).WithMessage("Workshop profile ID is required.");

            RuleFor(x => x)
                .MustAsync(async (model, cancellationToken) =>
                {
                    var existsResult = await _workingHoursService.WorkingHourExistsForDayAsync(model.WorkShopProfileId, model.Day, 0);
                    return !existsResult.Data;
                })
                .WithMessage("Working hours for this day already exist for this workshop.");
        }
    }
}