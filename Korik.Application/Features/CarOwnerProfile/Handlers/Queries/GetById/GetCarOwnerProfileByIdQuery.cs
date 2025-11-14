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
    public record GetCarOwnerProfileByIdQuery(GetCarOwnerProfileByIdDTO Model) : IRequest<ServiceResult<CarOwnerProfileDTO>> { }

    public class GetCarOwnerProfileByIdQueryHandler : IRequestHandler<GetCarOwnerProfileByIdQuery, ServiceResult<CarOwnerProfileDTO>>
    {
        #region Dependency Injection

        private readonly ICarOwnerProfileService _carOwnerProfileService;
        private readonly IMapper _mapper;
        private readonly IValidator<GetCarOwnerProfileByIdDTO> _validator;

        public GetCarOwnerProfileByIdQueryHandler
            (
            ICarOwnerProfileService carOwnerProfileService,
            IMapper mapper,
            IValidator<GetCarOwnerProfileByIdDTO> validator
            )
        {
            _carOwnerProfileService = carOwnerProfileService;
            _mapper = mapper;
            _validator = validator;
        }

        #endregion Dependency Injection

        public async Task<ServiceResult<CarOwnerProfileDTO>> Handle(GetCarOwnerProfileByIdQuery query, CancellationToken cancellationToken)
        {
            #region Not Valid

            var validationResult = await _validator.ValidateAsync(query.Model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(',', validationResult.Errors.Select(s => s.ErrorMessage));
                return ServiceResult<CarOwnerProfileDTO>.Fail(errors);
            }

            #endregion Not Valid

            #region Valid

            var result = await _carOwnerProfileService.GetByApplicationUserIdAsync(query.Model.ApplicationUserId);

            //result : Failed
            if (!result.Success)
            {
                return ServiceResult<CarOwnerProfileDTO>.Fail(result.Message ?? "not found");
            }

            //result : Success
            //Map Entity => DTO

            var carOwnerProfileDTO = _mapper.Map<CarOwnerProfileDTO>(result.Data);

            return ServiceResult<CarOwnerProfileDTO>.Ok(carOwnerProfileDTO);

            #endregion Valid
        }
    }
}