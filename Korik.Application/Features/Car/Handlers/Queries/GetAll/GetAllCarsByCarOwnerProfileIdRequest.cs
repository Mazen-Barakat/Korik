using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Korik.Application;
using MediatR;

namespace Korik.Application
{
    public record GetAllCarsByCarOwnerProfileIdRequest(GetCarsByCarOwnerProfileIdDTO Model) : IRequest<ServiceResult<IEnumerable<CarDTO>>>;

    public class GetAllCarsByCarOwnerProfileIdRequestHandler : IRequestHandler<GetAllCarsByCarOwnerProfileIdRequest, ServiceResult<IEnumerable<CarDTO>>>
    {
        private readonly ICarService _carService;
        private readonly IValidator<GetCarsByCarOwnerProfileIdDTO> _validator;
        private readonly IMapper _mapper;

        public GetAllCarsByCarOwnerProfileIdRequestHandler
            (ICarService carService, IValidator<GetCarsByCarOwnerProfileIdDTO> validator, IMapper mapper)
        {
            _carService = carService;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<ServiceResult<IEnumerable<CarDTO>>> Handle(GetAllCarsByCarOwnerProfileIdRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);

            #region Not Valid
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<IEnumerable<CarDTO>>.Fail(errors);
            }
            #endregion

            #region Valid
            var cars = await _carService.GetAllCarsByCarOwnerProfileIdAsync(request.Model.CarOwnerProfileId);

            var carDTOs = _mapper.Map<IEnumerable<CarDTO>>(cars.Data);
            #endregion

            return ServiceResult<IEnumerable<CarDTO>>.Ok(carDTOs);
        }
    }
}
