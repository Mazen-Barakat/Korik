using AutoMapper;
using FluentValidation;
using Korik.Application;
using Korik.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Korik.Application
{
    public record CreateCarOwnerProfileRequest(CreateCarOwnerProfileDTO Model) : IRequest<ServiceResult<CarOwnerProfileDTO>> { }

    public class CreateCarOwnerProfileRequestHandler : IRequestHandler<CreateCarOwnerProfileRequest, ServiceResult<CarOwnerProfileDTO>>
    {
        #region Dependency Injection

        private readonly ICarOwnerProfileService _carOwnerProfileService;
        private readonly IValidator<CreateCarOwnerProfileDTO> _validator;
        private readonly IMapper _mapper;

        public CreateCarOwnerProfileRequestHandler
            (
            ICarOwnerProfileService carOwnerProfileService,
            IValidator<CreateCarOwnerProfileDTO> validator,
            IMapper mapper
            )
        {
            _carOwnerProfileService = carOwnerProfileService;
            _validator = validator;
            _mapper = mapper;
        }

        #endregion Dependency Injection

        //test1
        public async Task<ServiceResult<CarOwnerProfileDTO>> Handle(CreateCarOwnerProfileRequest request, CancellationToken cancellationToken)
        {
            #region Not Valid

            var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);

            if (!validationResult.IsValid)
            {
                var erors = string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<CarOwnerProfileDTO>.Fail(erors);
            }

            #endregion Not Valid

            //Map Request => Entity

            var carOwnerProfile = _mapper.Map<CarOwnerProfile>(request.Model);

            var result = await _carOwnerProfileService.CreateAsync(carOwnerProfile);

            //UpdateResult : Faild
            if (!result.Success)
                return ServiceResult<CarOwnerProfileDTO>.Fail(result.Message ?? "Failed to create Car Owner Profile.");

            //createResult : Success
            //Map Entity => DTO
            var carOwnerProfileDTO = _mapper.Map<CarOwnerProfileDTO>(result.Data);

            return ServiceResult<CarOwnerProfileDTO>.Created(carOwnerProfileDTO);
        }
    }
}