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
    public record GetWorkShopProfileByIdRequest(GetWorkShopProfileByIdDTO Model) : IRequest<ServiceResult<WorkShopProfileDTO>>
    {
    }

    public class GetWorkShopProfileByIdRequestHandler : IRequestHandler<GetWorkShopProfileByIdRequest, ServiceResult<WorkShopProfileDTO>>
    {
        private readonly IWorkShopProfileService _workShopProfileService;
        private readonly IMapper _mapper;
        private readonly IValidator<GetWorkShopProfileByIdDTO> _validator;

        public GetWorkShopProfileByIdRequestHandler
            (
            IWorkShopProfileService workShopProfileService,
            IMapper mapper,
            IValidator<GetWorkShopProfileByIdDTO> validator
            )
        {
            _workShopProfileService = workShopProfileService;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<ServiceResult<WorkShopProfileDTO>> Handle(GetWorkShopProfileByIdRequest request, CancellationToken cancellationToken)
        {
            #region Not Valid

            var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<WorkShopProfileDTO>.Fail(errors);
            }

            #endregion Not Valid

            #region Valid

            var result = await _workShopProfileService.GetByIdAsync(request.Model.Id);

            //result : Failed
            if (!result.Success)
            {
                return ServiceResult<WorkShopProfileDTO>.Fail(result.Message ?? "not found");
            }

            //result : Success
            //Map Entity => DTO

            var workShopProfileDTO = _mapper.Map<WorkShopProfileDTO>(result.Data);

            return ServiceResult<WorkShopProfileDTO>.Ok(workShopProfileDTO);

            #endregion Valid
        }
    }
}