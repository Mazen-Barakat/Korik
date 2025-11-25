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
    public record CreateWorkshopServiceRequest(CreateWorkshopServiceDTO Model) : IRequest<ServiceResult<WorkshopServiceDTO>> { }

    public class CreateWorkshopServiceRequestHandler : IRequestHandler<CreateWorkshopServiceRequest, ServiceResult<WorkshopServiceDTO>>
    {
        #region Dependency Injection

        private readonly IWorkshopServiceService _workshopServiceService;
        private readonly IValidator<CreateWorkshopServiceDTO> _validator;
        private readonly IMapper _mapper;

        public CreateWorkshopServiceRequestHandler
            (
            IWorkshopServiceService workshopServiceService,
            IValidator<CreateWorkshopServiceDTO> validator,
            IMapper mapper
            )
        {
            _workshopServiceService = workshopServiceService;
            _validator = validator;
            _mapper = mapper;
        }

        #endregion Dependency Injection

        public async Task<ServiceResult<WorkshopServiceDTO>> Handle(CreateWorkshopServiceRequest request, CancellationToken cancellationToken)
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

            //Map Request => Entity

            var workshopService = _mapper.Map<WorkshopService>(request.Model);

            var result = await _workshopServiceService.CreateAsync(workshopService);

            //result : Faild
            if (!result.Success)
                return ServiceResult<WorkshopServiceDTO>.Fail(result.Message ?? "Failed to create service for  this Workshop.");

            //result : Success
            //Map Entity => DTO
            var workshopServiceDTO = _mapper.Map<WorkshopServiceDTO>(result.Data);

            return ServiceResult<WorkshopServiceDTO>.Created(workshopServiceDTO);

            #endregion Valid
        }
    }
}