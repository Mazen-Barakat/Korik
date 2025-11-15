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
    public record UpdateCarIndicatorRequest(UpdateCarIndicatorDTO Model) : IRequest<ServiceResult<CarIndicatorDTO>>;

    public class UpdateCarIndicatorRequestHandler : IRequestHandler<UpdateCarIndicatorRequest, ServiceResult<CarIndicatorDTO>>
    {
        private readonly ICarIndicatorService _carIndicatorService;
        private readonly IValidator<UpdateCarIndicatorDTO> _validator;
        private IMapper _mapper;

        public UpdateCarIndicatorRequestHandler
            (ICarIndicatorService carIndicatorService,
            IValidator<UpdateCarIndicatorDTO> validator,
            IMapper mapper)
        {
            _carIndicatorService = carIndicatorService;
            _validator = validator;
            _mapper = mapper;   
        }
        public async Task<ServiceResult<CarIndicatorDTO>> Handle(UpdateCarIndicatorRequest request, CancellationToken cancellationToken)
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

            var updatedCarIndicatorResult = await _carIndicatorService.UpdateAsync(_mapper.Map<CarIndicator>(request.Model));

            if(!updatedCarIndicatorResult.Success)
            {
                return ServiceResult<CarIndicatorDTO>.Fail(updatedCarIndicatorResult.Message?? "Failed to update car indicator.");
            }

            #endregion

            return ServiceResult<CarIndicatorDTO>.Ok(_mapper.Map<CarIndicatorDTO>(updatedCarIndicatorResult.Data), "Car indicator updated successfully.");

        }
    }
}
