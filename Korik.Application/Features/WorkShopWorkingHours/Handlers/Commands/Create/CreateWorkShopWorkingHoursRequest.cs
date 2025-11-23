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
    public record CreateWorkShopWorkingHoursRequest(CreateWorkShopWorkingHoursDTO model) : IRequest<ServiceResult<WorkShopWorkingHoursDTO>> { }

    public class CreateWorkShopWorkingHoursRequestHandler : IRequestHandler<CreateWorkShopWorkingHoursRequest, ServiceResult<WorkShopWorkingHoursDTO>>
    {
        private readonly IWorkShopWorkingHoursService _workingHoursService;
        private readonly IValidator<CreateWorkShopWorkingHoursDTO> _validator;
        private readonly IMapper _mapper;

        public CreateWorkShopWorkingHoursRequestHandler(
            IWorkShopWorkingHoursService workingHoursService,
            IValidator<CreateWorkShopWorkingHoursDTO> validator,
            IMapper mapper)
        {
            _workingHoursService = workingHoursService;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<ServiceResult<WorkShopWorkingHoursDTO>> Handle(CreateWorkShopWorkingHoursRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<WorkShopWorkingHoursDTO>.Fail(errors);
            }

            var workingHours = _mapper.Map<WorkingHours>(request.model);

            var createdResult = await _workingHoursService.CreateAsync(workingHours);

            if (!createdResult.Success)
            {
                return ServiceResult<WorkShopWorkingHoursDTO>.Fail(createdResult.Message ?? "Failed to create working hours.");
            }

            var workingHoursDTO = _mapper.Map<WorkShopWorkingHoursDTO>(createdResult.Data);

            return ServiceResult<WorkShopWorkingHoursDTO>.Created(workingHoursDTO, "Working hours created successfully.");
        }
    }
}