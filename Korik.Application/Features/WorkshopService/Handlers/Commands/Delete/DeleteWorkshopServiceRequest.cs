using AutoMapper;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record DeleteWorkshopServiceRequest(DeleteWorkshopServiceDTO Model) : IRequest<ServiceResult<WorkshopServiceDTO>> { }

    public class DeleteWorkshopServiceRequestHandler : IRequestHandler<DeleteWorkshopServiceRequest, ServiceResult<WorkshopServiceDTO>>
    {
        #region Dependency Injection

        private readonly IWorkshopServiceService _workshopServiceService;
        private readonly IValidator<DeleteWorkshopServiceDTO> _validator;
        private readonly IMapper _mapper;

        public DeleteWorkshopServiceRequestHandler
            (
            IWorkshopServiceService workshopServiceService,
            IValidator<DeleteWorkshopServiceDTO> validator,
            IMapper mapper
            )
        {
            _workshopServiceService = workshopServiceService;
            _validator = validator;
            _mapper = mapper;
        }

        #endregion Dependency Injection

        public async Task<ServiceResult<WorkshopServiceDTO>> Handle(DeleteWorkshopServiceRequest request, CancellationToken cancellationToken)
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
            var result = await _workshopServiceService.DeleteAsync(request.Model.Id);

            //result : Faild
            if (!result.Success)
            {
                return ServiceResult<WorkshopServiceDTO>.Fail(result.Message ?? "Failed to delete category.");
            }

            //result : Success
            //Map Entity => DTO
            var workshopServiceDTO = _mapper.Map<WorkshopServiceDTO>(result.Data);

            return ServiceResult<WorkshopServiceDTO>.Ok(workshopServiceDTO);

            #endregion Valid
        }
    }
}