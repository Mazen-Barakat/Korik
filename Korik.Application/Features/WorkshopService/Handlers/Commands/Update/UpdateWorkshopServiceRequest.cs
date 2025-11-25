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
    public record UpdateWorkshopServiceRequest(UpdateWorkshopServiceDTO Model) : IRequest<ServiceResult<WorkshopServiceDTO>> { }

    public class UpdateWorkshopServiceRequestHandler : IRequestHandler<UpdateWorkshopServiceRequest, ServiceResult<WorkshopServiceDTO>>
    {
        #region Dependency Injection

        private readonly IWorkshopServiceService _workshopServiceService;
        private readonly IValidator<UpdateWorkshopServiceDTO> _validator;
        private readonly IMapper _mapper;

        public UpdateWorkshopServiceRequestHandler
            (
            IWorkshopServiceService workshopServiceService,
            IValidator<UpdateWorkshopServiceDTO> validator,
            IMapper mapper
            )
        {
            _workshopServiceService = workshopServiceService;
            _validator = validator;
            _mapper = mapper;
        }

        #endregion Dependency Injection

        public async Task<ServiceResult<WorkshopServiceDTO>> Handle(UpdateWorkshopServiceRequest request, CancellationToken cancellationToken)
        {
            #region Not Valid

            var validationResult = await _validator.ValidateAsync
                (
                request.Model,
                cancellationToken
                );

            if (!validationResult.IsValid)
            {
                var errors = string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<WorkshopServiceDTO>.Fail(errors);
            }

            #endregion Not Valid

            #region Valid

            // Get existing entity
            var existingServiceResult = await _workshopServiceService.GetByIdAsync(request.Model.Id);

            if (!existingServiceResult.Success || existingServiceResult.Data == null)
                return ServiceResult<WorkshopServiceDTO>.Fail("Workshop service not found.");

            // Map ONTO the existing entity (this preserves unmapped properties)
            _mapper.Map(request.Model, existingServiceResult.Data);

            // Update
            var result = await _workshopServiceService.UpdateAsync(existingServiceResult.Data);

            if (!result.Success)
                return ServiceResult<WorkshopServiceDTO>.Fail(result.Message ?? "Failed to update service.");

            var workshopServiceDTO = _mapper.Map<WorkshopServiceDTO>(result.Data);
            return ServiceResult<WorkshopServiceDTO>.Ok(workshopServiceDTO);

            #endregion Valid
        }
    }
}