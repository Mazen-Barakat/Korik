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
        private readonly IWorkShopProfileService _workShopProfileService;

        public CreateWorkShopWorkingHoursDTOValidator
            (
            IWorkShopWorkingHoursService workingHoursService, 
            IWorkShopProfileService workShopProfileService
            )
        {
            _workingHoursService = workingHoursService;
            _workShopProfileService = workShopProfileService;


            RuleFor(x => x.Day)
                .IsInEnum().WithMessage("Day must be a valid day of the week.");

            RuleFor(x => x.From)
                .NotEmpty().WithMessage("From time is required.");

            RuleFor(x => x.To)
                .NotEmpty().WithMessage("To time is required.")
                .Must((model, to) => model.IsClosed || to > model.From)
                .WithMessage("To time must be after From time.");

            RuleFor(x => x.WorkShopProfileId)
                .GreaterThan(0).WithMessage("Workshop profile ID is required.")
                .MustAsync(async (id, cancellationToken) =>
                {
                    var result = await _workShopProfileService.IsExistAsync(id);
                    return result.Data;   // must be bool
                })
                .WithMessage("Workshop profile does not exist.");

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