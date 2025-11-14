using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Korik.Domain;

namespace Korik.Application
{
    public record UpdateCarExpenseRequest(UpdateCarExpenseDTO Model) : IRequest<ServiceResult<CarExpenseDTO>>;

    public class UpdateCarExpenseRequestHandler : IRequestHandler<UpdateCarExpenseRequest, ServiceResult<CarExpenseDTO>>
    {
        private readonly ICarExpenseService _service;
        private readonly IMapper _mapper;
        private readonly IValidator<UpdateCarExpenseDTO> _validator;

        public UpdateCarExpenseRequestHandler(
            ICarExpenseService service,
            IMapper mapper,
            IValidator<UpdateCarExpenseDTO> validator)
        {
            _service = service;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<ServiceResult<CarExpenseDTO>> Handle(UpdateCarExpenseRequest request, CancellationToken cancellationToken)
        {
            #region Not Valid
            // Validate DTO
            var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<CarExpenseDTO>.Fail(errors);
            }

            #endregion

            #region Valid
            // Map updated properties
            // Update entity

            var updateResult = await _service.UpdateAsync(_mapper.Map<CarExpenses>(request.Model));

            if (!updateResult.Success)
            {
                return ServiceResult<CarExpenseDTO>.Fail(updateResult.Message ?? "Failed to update car expense.");
            }

            // Map updated entity to DTO
            var carExpenseDto = _mapper.Map<CarExpenseDTO>(updateResult.Data);

            #endregion

            return ServiceResult<CarExpenseDTO>.Ok(carExpenseDto, "Car expense updated successfully.");
        }
    }
}
