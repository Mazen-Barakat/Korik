using AutoMapper;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Korik.Application
{
    public record GetCarOwnerProfileByIdWithCarQuery(GetCarOwnerProfileByIdDTO Model) : IRequest<ServiceResult<CarOwnerProfileWithCarDTO>> { }

    public class GetCarOwnerProfileByIdWithCarQueryHandler : IRequestHandler<GetCarOwnerProfileByIdWithCarQuery, ServiceResult<CarOwnerProfileWithCarDTO>>
    {
        #region Dependency Injection

        private readonly ICarOwnerProfileService _carOwnerProfileService;
        private readonly IMapper _mapper;
        private readonly IValidator<GetCarOwnerProfileByIdDTO> _validator;

        public GetCarOwnerProfileByIdWithCarQueryHandler
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

        public async Task<ServiceResult<CarOwnerProfileWithCarDTO>> Handle(GetCarOwnerProfileByIdWithCarQuery query, CancellationToken cancellationToken)
        {
            #region Not Valid

            var validationResult = await _validator.ValidateAsync(query.Model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(',', validationResult.Errors.Select(s => s.ErrorMessage));
                return ServiceResult<CarOwnerProfileWithCarDTO>.Fail(errors);
            }

            #endregion Not Valid

            #region Valid

            var result = await _carOwnerProfileService.GetByApplicationUserIdWithIncludeAsync
                (
                query.Model.ApplicationUserId,
                c => c.Cars
                );

            if (!result.Success)
            {
                return ServiceResult<CarOwnerProfileWithCarDTO>.Fail(result.Message ?? "not found");
            }

            var carOwnerProfileWithCarDTO = _mapper.Map<CarOwnerProfileWithCarDTO>(result.Data);

            return ServiceResult<CarOwnerProfileWithCarDTO>.Ok(carOwnerProfileWithCarDTO);

            #endregion Valid
        }
    }
}