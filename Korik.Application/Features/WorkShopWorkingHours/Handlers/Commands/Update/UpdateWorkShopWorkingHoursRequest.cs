using AutoMapper;
using FluentValidation;
using Korik.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record UpdateWorkShopWorkingHoursRequest(UpdateWorkShopWorkingHoursDTO model) : IRequest<ServiceResult<WorkShopWorkingHoursDTO>> { }

    public class UpdateWorkShopWorkingHoursRequestHandler : IRequestHandler<UpdateWorkShopWorkingHoursRequest, ServiceResult<WorkShopWorkingHoursDTO>>
    {
        private readonly IWorkShopWorkingHoursService _workingHoursService;
        private readonly IValidator<UpdateWorkShopWorkingHoursDTO> _validator;
        private readonly IMapper _mapper;

        public UpdateWorkShopWorkingHoursRequestHandler(
            IWorkShopWorkingHoursService workingHoursService,
            IValidator<UpdateWorkShopWorkingHoursDTO> validator,
            IMapper mapper)
        {
            _workingHoursService = workingHoursService;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<ServiceResult<WorkShopWorkingHoursDTO>> Handle(UpdateWorkShopWorkingHoursRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<WorkShopWorkingHoursDTO>.Fail(errors);
            }

            var existsResult = await _workingHoursService.IsExistAsync(request.model.Id);
            if (!existsResult.Data)
            {
                return ServiceResult<WorkShopWorkingHoursDTO>.Fail("Working hours not found.");
            }

            var workingHours = _mapper.Map<WorkingHours>(request.model);

            var updatedResult = await _workingHoursService.UpdateAsync(workingHours);

            if (!updatedResult.Success)
            {
                return ServiceResult<WorkShopWorkingHoursDTO>.Fail(updatedResult.Message ?? "Failed to update working hours.");
            }

            var workingHoursDTO = _mapper.Map<WorkShopWorkingHoursDTO>(updatedResult.Data);

            return ServiceResult<WorkShopWorkingHoursDTO>.Ok(workingHoursDTO, "Working hours updated successfully.");
        }
    }
}