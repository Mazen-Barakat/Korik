using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Korik.Domain;

namespace Korik.Application
{
    public record UpdateCarRequest(UpdateCarDTO Model) : IRequest<ServiceResult<CarDTO>>;

    public class UpdateCarRequestHandler : IRequestHandler<UpdateCarRequest, ServiceResult<CarDTO>>
    {
        private readonly ICarService _service;
        private readonly IMapper _mapper;
        private readonly IValidator<UpdateCarDTO> _validator;

        public UpdateCarRequestHandler(
            ICarService service,
            IMapper mapper,
            IValidator<UpdateCarDTO> validator)
        {
            _service = service;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<ServiceResult<CarDTO>> Handle(UpdateCarRequest request, CancellationToken cancellationToken)
        {
            // Validate DTO
            var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);

            #region Not Valid
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<CarDTO>.Fail(errors);
            }

            #endregion

            #region Valid

            // Retrieve the existing entity to ensure it is tracked
            var isEntityExistsResult = await _service.GetByIdAsync(request.Model.Id);

            if (!isEntityExistsResult.Success)
            {
                return ServiceResult<CarDTO>.Fail(isEntityExistsResult.Message ?? "Entity not found.");
            }

            // Map updated properties to the existing entity
            var updatedCar = _mapper.Map<Car>(request.Model);

            // Update entity
            var updateResult = await _service.UpdateAsync(updatedCar);

            if (!updateResult.Success)
            {
                return ServiceResult<CarDTO>.Fail(updateResult.Message ?? "Failed to update car.");
            }

            // Map updated entity to DTO
            var carDto = _mapper.Map<CarDTO>(updateResult.Data);

            #endregion

            return ServiceResult<CarDTO>.Ok(carDto, "Car updated successfully.");
        }
    }
}
