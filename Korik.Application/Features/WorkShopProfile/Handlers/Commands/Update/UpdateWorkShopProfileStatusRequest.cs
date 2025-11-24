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
    public record UpdateWorkShopProfileStatusRequest(UpdateWorkShopProfileStatusDTO Model) : IRequest<ServiceResult<WorkShopProfileDTO>> { }

    public class UpdateWorkShopProfileStatusRequestHanler : IRequestHandler<UpdateWorkShopProfileStatusRequest, ServiceResult<WorkShopProfileDTO>>
    {
        #region Dependency Injection

        private readonly IWorkShopProfileService _workShopProfileService;
        private readonly IValidator<UpdateWorkShopProfileStatusDTO> _validator;
        private readonly IMapper _mapper;

        public UpdateWorkShopProfileStatusRequestHanler
            (
            IWorkShopProfileService workShopProfileService,
            IValidator<UpdateWorkShopProfileStatusDTO> validator,
            IMapper mapper
            )
        {
            _workShopProfileService = workShopProfileService;
            _validator = validator;
            _mapper = mapper;
        }

        #endregion Dependency Injection

        public async Task<ServiceResult<WorkShopProfileDTO>> Handle(UpdateWorkShopProfileStatusRequest request, CancellationToken cancellationToken)
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

            var existingWorkShopProfile = await _workShopProfileService.GetByIdAsync(request.Model.Id);
            if (existingWorkShopProfile?.Data == null)
                return ServiceResult<WorkShopProfileDTO>.Fail("Car Owner Profile not found.");

            _mapper.Map(request.Model, existingWorkShopProfile.Data);

            var result = await _workShopProfileService.UpdateAsync(existingWorkShopProfile.Data);

            // UpdateResult : Faild
            if (!result.Success)
                return ServiceResult<WorkShopProfileDTO>.Fail(result.Message ?? "Failed to Update Car Owner Status Profile.");

            //createResult : Success
            //Map Entity => DTO

            var workShopProfileDTO = _mapper.Map<WorkShopProfileDTO>(result.Data);

            return ServiceResult<WorkShopProfileDTO>.Ok(workShopProfileDTO);

            #endregion Valid
        }
    }
}