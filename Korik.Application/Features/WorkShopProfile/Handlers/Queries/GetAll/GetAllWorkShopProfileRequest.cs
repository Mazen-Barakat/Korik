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
    public record GetAllWorkShopProfileRequest(PagedRequestDTO Model) : IRequest<ServiceResult<PagedResult<WorkShopProfileDTO>>> { }

    public class GetAllWorkShopProfileRequestHandler : IRequestHandler<GetAllWorkShopProfileRequest, ServiceResult<PagedResult<WorkShopProfileDTO>>>
    {
        #region Dependency Injection

        private readonly IWorkShopProfileService _workShopProfileService;
        private readonly IMapper _mapper;
        private readonly IValidator<PagedRequestDTO> _validator;

        public GetAllWorkShopProfileRequestHandler(
            IWorkShopProfileService workShopProfileService,
            IMapper mapper,
            IValidator<PagedRequestDTO> validator
            )
        {
            _workShopProfileService = workShopProfileService;
            _mapper = mapper;
            _validator = validator;
        }

        #endregion Dependency Injection

        public async Task<ServiceResult<PagedResult<WorkShopProfileDTO>>> Handle(GetAllWorkShopProfileRequest request, CancellationToken cancellationToken)
        {
            #region Not Valid

            var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<PagedResult<WorkShopProfileDTO>>.Fail(errors);
            }

            #endregion Not Valid

            #region Valid

            var result = await _workShopProfileService.GetAllPagedAsync
                (
               request.Model.PageNumber,
                request.Model.PageSize,
                   x => x.VerificationStatus == VerificationStatus.Verified
                );

            //Not Valid
            if (!result.Success)
            {
                return ServiceResult<PagedResult<WorkShopProfileDTO>>.Fail($"Bad Request - Error While getting Data: {result.Message}");
            }

            //Valid
            //Map Entity(IEnumerable) => DTO (IEnumerable)
            var pagedResult = _mapper.Map<PagedResult<WorkShopProfileDTO>>(result.Data);

            return ServiceResult<PagedResult<WorkShopProfileDTO>>.Ok(pagedResult);

            #endregion Valid
        }
    }
}