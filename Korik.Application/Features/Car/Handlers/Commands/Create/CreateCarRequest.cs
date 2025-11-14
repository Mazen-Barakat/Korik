using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Korik.Domain;

namespace Korik.Application
{
    public record CreateCarRequest(CreateCarDTO Model) : IRequest<ServiceResult<CarDTO>>;

    public class CreateCarRequestHandler : IRequestHandler<CreateCarRequest, ServiceResult<CarDTO>>
    {
        private readonly ICarService _service;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateCarDTO> _validator;

        public CreateCarRequestHandler(
            ICarService service,
            IMapper mapper,
            IValidator<CreateCarDTO> validator)
        {
            _service = service;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<ServiceResult<CarDTO>> Handle(CreateCarRequest request, CancellationToken cancellationToken)
        {
            #region Not Valid
            var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<CarDTO>.Fail(errors);
            }
            #endregion


            #region Valid
            // Map DTO -> Entity
            var carEntity = _mapper.Map<Car>(request.Model);

            // Create
            var createResult = await _service.CreateAsync(carEntity);
            if (!createResult.Success)
                return ServiceResult<CarDTO>.Fail(createResult.Message ?? "Failed to create car.");

            // Map Entity -> DTO
            var carDto = _mapper.Map<CarDTO>(createResult.Data); 
            #endregion

            return ServiceResult<CarDTO>.Created(carDto);
        }
    }
}
