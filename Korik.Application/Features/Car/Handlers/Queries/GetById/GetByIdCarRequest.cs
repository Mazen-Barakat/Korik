using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Korik.Domain;

namespace Korik.Application
{
    public record GetByIdCarRequest(GetByIdCarDTO Model) : IRequest<ServiceResult<CarDTO>>;

    public class GetByIdCarRequestHandler : IRequestHandler<GetByIdCarRequest, ServiceResult<CarDTO>>
    {
        private readonly ICarService _service;
        private readonly IValidator<GetByIdCarDTO> _validator;
        private readonly IMapper _mapper;

        public GetByIdCarRequestHandler(
            ICarService service,
            IValidator<GetByIdCarDTO> validator,
            IMapper mapper)
        {
            _service = service;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<ServiceResult<CarDTO>> Handle(GetByIdCarRequest request, CancellationToken cancellationToken)
        {
            #region Not Valid
            // Validate DTO
            var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<CarDTO>.Fail(errors);
            }

            #endregion

            #region Valid
            // Retrieve car by Id
            var carResult = await _service.GetByIdAsync(request.Model.Id);

            if (!carResult.Success || carResult.Data == null)
            {
                return ServiceResult<CarDTO>.Fail(carResult.Message?? "Car not found.");
            }

            // Map to DTO
            var carDto = _mapper.Map<CarDTO>(carResult.Data);

            #endregion

            return ServiceResult<CarDTO>.Ok(carDto);
        }
    }
}
