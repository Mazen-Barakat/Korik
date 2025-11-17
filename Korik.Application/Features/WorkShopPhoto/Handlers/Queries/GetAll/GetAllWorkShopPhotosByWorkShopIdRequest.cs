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
    public record GetAllWorkShopPhotosByWorkShopIdRequest(GetAllWorkShopPhotosByWorkShopIdDTO Model)
        : IRequest<ServiceResult<IEnumerable<WorkShopPhotoItemDTO>>>
    {
    }

    public class GetAllWorkShopPhotosByWorkShopIdRequestHandler
        : IRequestHandler<GetAllWorkShopPhotosByWorkShopIdRequest, ServiceResult<IEnumerable<WorkShopPhotoItemDTO>>>
    {
        #region Dependency Injection

        private readonly IWorkShopPhotoService _workShopPhotoService;
        private readonly IMapper _mapper;
        private readonly IValidator<GetAllWorkShopPhotosByWorkShopIdDTO> _validator;

        public GetAllWorkShopPhotosByWorkShopIdRequestHandler(
            IWorkShopPhotoService workShopPhotoService,
            IMapper mapper,
            IValidator<GetAllWorkShopPhotosByWorkShopIdDTO> validator
            )
        {
            _workShopPhotoService = workShopPhotoService;
            _mapper = mapper;
            _validator = validator;
        }

        #endregion Dependency Injection

        public async Task<ServiceResult<IEnumerable<WorkShopPhotoItemDTO>>> Handle(GetAllWorkShopPhotosByWorkShopIdRequest request, CancellationToken cancellationToken)
        {
            #region Not Valid

            var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<IEnumerable<WorkShopPhotoItemDTO>>.Fail(errors);
            }

            #endregion Not Valid

            var result = await _workShopPhotoService.GetAllPhotosByWorkShopIdAsync(request.Model.WorkShopProfileId);

            //Not Valid
            if (!result.Success)
            {
                return ServiceResult<IEnumerable<WorkShopPhotoItemDTO>>.Fail($"Bad Request - Error While getting Data: {result.Message}");
            }

            //Valid
            //Map Entity(IEnumerable) => DTO (IEnumerable)
            var workShopPhotoItemDTO = _mapper.Map<IEnumerable<WorkShopPhotoItemDTO>>(result.Data);

            return ServiceResult<IEnumerable<WorkShopPhotoItemDTO>>.Ok(workShopPhotoItemDTO);
        }
    }
}