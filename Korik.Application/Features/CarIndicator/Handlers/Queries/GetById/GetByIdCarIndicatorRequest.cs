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
    public record GetByIdCarIndicatorRequest(GetByIdCarIndicatorDTO Model) : IRequest<ServiceResult<CarIndicatorDTO>>;

    public class GetByIdCarIndicatorRequestHandler : IRequestHandler<GetByIdCarIndicatorRequest, ServiceResult<CarIndicatorDTO>>
    {
        private readonly ICarIndicatorService _carIndicatorService;
        private readonly IValidator<GetByIdCarIndicatorDTO> _validator;
        private readonly IMapper _mapper;

        public GetByIdCarIndicatorRequestHandler
            (ICarIndicatorService carIndicatorService, 
            IValidator<GetByIdCarIndicatorDTO> validator, 
            IMapper mapper)
        {
            _carIndicatorService = carIndicatorService;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<ServiceResult<CarIndicatorDTO>> Handle(GetByIdCarIndicatorRequest request, CancellationToken cancellationToken)
        {
            #region Not Valid
            var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ServiceResult<CarIndicatorDTO>.Fail(string.Join("; ", errors));
            }

            #endregion

            #region Valid

            var carIndicatorResult = await _carIndicatorService.GetByIdAsync(request.Model.Id);

            if (!carIndicatorResult.Success)
            {
                return ServiceResult<CarIndicatorDTO>.Fail(carIndicatorResult.Message ?? "Car indicator not found.");
            }

            #endregion


            return ServiceResult<CarIndicatorDTO>.Ok(_mapper.Map<CarIndicatorDTO>(carIndicatorResult.Data));

        }
    }

}
