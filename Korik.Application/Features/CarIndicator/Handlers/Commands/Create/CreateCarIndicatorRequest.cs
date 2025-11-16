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
    public record CreateCarIndicatorRequest(CreateCarIndicatorDTO Model) : IRequest<ServiceResult<CarIndicatorDTO>>;

    public class CreateCarIndicatorRequestHandler : IRequestHandler<CreateCarIndicatorRequest, ServiceResult<CarIndicatorDTO>>
    {
        private readonly ICarIndicatorService _carIndicatorService;
        private readonly IValidator<CreateCarIndicatorDTO> _validator;
        private readonly IMapper _mapper;

        public CreateCarIndicatorRequestHandler(
            ICarIndicatorService carIndicatorService,
            IValidator<CreateCarIndicatorDTO> validator,
            IMapper mapper)
        {
            _carIndicatorService = carIndicatorService;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<ServiceResult<CarIndicatorDTO>> Handle(CreateCarIndicatorRequest request, CancellationToken cancellationToken)
        {
            // Validate the request
            var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<CarIndicatorDTO>.Fail(errors);
            }

            // Map the DTO to the domain model
            var carIndicator = _mapper.Map<CarIndicator>(request.Model);

            // Call the service to create the car indicator
            var result = await _carIndicatorService.CreateAsync(carIndicator);
            if (!result.Success)
            {
                return ServiceResult<CarIndicatorDTO>.Fail(result.Message ?? "Unable to create car indicator.");
            }

            // Map the created domain model back to the DTO
            var createdCarIndicatorDTO = _mapper.Map<CarIndicatorDTO>(result.Data);

            return ServiceResult<CarIndicatorDTO>.Created(createdCarIndicatorDTO);
        }
    }
}
