using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record GetWorkshopServicesByProfileIDRequest(GetWorkshopServicesByProfileIDDTO Model) :
        IRequest<ServiceResult<PagedResult<WorkshopServiceWithServiceDTO>>>
    {
    }

    public class GetWorkshopServicesByProfileIDRequestHandler : IRequestHandler<GetWorkshopServicesByProfileIDRequest, ServiceResult<PagedResult<WorkshopServiceWithServiceDTO>>>
    {
        #region Dependency Injection

        private readonly IWorkshopServiceService _workshopServiceService;
        private readonly IValidator<GetWorkshopServicesByProfileIDDTO> _validator;
        private readonly IMapper _mapper;

        public GetWorkshopServicesByProfileIDRequestHandler
            (
            IWorkshopServiceService workshopServiceService,
            IValidator<GetWorkshopServicesByProfileIDDTO> validator,
            IMapper mapper
            )
        {
            _workshopServiceService = workshopServiceService;
            _validator = validator;
            _mapper = mapper;
        }

        #endregion Dependency Injection

        public async Task<ServiceResult<PagedResult<WorkshopServiceWithServiceDTO>>> Handle(GetWorkshopServicesByProfileIDRequest request, CancellationToken cancellationToken)
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
                return ServiceResult<PagedResult<WorkshopServiceWithServiceDTO>>.Fail(errors);
            }

            #endregion Not Valid

            #region Valid

            var result = await _workshopServiceService.GetAllPagedAsync
                (
               request.Model.PageNumber,
               request.Model.PageSize,
            x => x.WorkShopProfileId == request.Model.Id,
            x => x.Service
                );

            //Not Valid
            if (!result.Success)
            {
                return ServiceResult<PagedResult<WorkshopServiceWithServiceDTO>>.Fail($"Bad Request - Error While getting Data: {result.Message}");
            }

            //Valid
            //Map Entity(IEnumerable) => DTO (IEnumerable)
            var pagedResult = _mapper.Map<PagedResult<WorkshopServiceWithServiceDTO>>(result.Data);

            return ServiceResult<PagedResult<WorkshopServiceWithServiceDTO>>.Ok(pagedResult);

            #endregion Valid
        }
    }
}