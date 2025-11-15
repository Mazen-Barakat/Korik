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
    public record GetAllCarIndicatorsByCarIdRequest (GetAllIndicatorsByCarIdDTO Model) : IRequest<ServiceResult<IEnumerable<CarIndicatorDTO>>>;
    
    public class GetAllCarIndicatorsByCarIdRequestHandler : IRequestHandler<GetAllCarIndicatorsByCarIdRequest, ServiceResult<IEnumerable<CarIndicatorDTO>>>
    {
        private readonly ICarIndicatorService _carIndicatorService;
        private readonly IValidator<GetAllIndicatorsByCarIdDTO> _validator;
        private readonly IMapper _mapper;
    
        public GetAllCarIndicatorsByCarIdRequestHandler(
            ICarIndicatorService carIndicatorService,
            IValidator<GetAllIndicatorsByCarIdDTO> validator,
            IMapper mapper)
        {
            _carIndicatorService = carIndicatorService;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<ServiceResult<IEnumerable<CarIndicatorDTO>>> Handle(GetAllCarIndicatorsByCarIdRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);

            #region Not Valid
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<IEnumerable<CarIndicatorDTO>>.Fail(errors);
            }
            #endregion
    
            #region Valid
                var carIndicatorsResult = await _carIndicatorService.GetAllCarIndicatorsByCarId(request.Model.CarId);

                if(!carIndicatorsResult.Success)
                {
                    return ServiceResult<IEnumerable<CarIndicatorDTO>>.Fail(carIndicatorsResult.Message ?? "Failed to retrieve car indicators.");
                }

                #endregion
    
            return ServiceResult<IEnumerable<CarIndicatorDTO>>.Ok(_mapper.Map<IEnumerable<CarIndicatorDTO>>(carIndicatorsResult.Data));
        }
    }
}
