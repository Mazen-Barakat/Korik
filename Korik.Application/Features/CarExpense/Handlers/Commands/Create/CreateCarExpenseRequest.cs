using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Korik.Domain;
using Korik.Application.DTOs.CarExpenses.Request_DTOs.CreateCarExpenseDTO;

namespace Korik.Application
{
    public record CreateCarExpenseRequest(CreateCarExpanseDTO Model) : IRequest<ServiceResult<CarExpenseDTO>>;

    public class CreateCarExpenseRequestHandler : IRequestHandler<CreateCarExpenseRequest, ServiceResult<CarExpenseDTO>>
    {
        private readonly ICarExpenseService _service;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateCarExpanseDTO> _validator;

        public CreateCarExpenseRequestHandler(
            ICarExpenseService service,
            IMapper mapper,
            IValidator<CreateCarExpanseDTO> validator)
        {
            _service = service;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<ServiceResult<CarExpenseDTO>> Handle(CreateCarExpenseRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);

            #region Not Valid
            // Validate DTO
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<CarExpenseDTO>.Fail(errors);
            }

            #endregion

            #region Valid
            // Map DTO -> Entity
            var carExpenseEntity = _mapper.Map<CarExpenses>(request.Model);

            // Create
            var createResult = await _service.CreateAsync(carExpenseEntity);

            // Map Entity -> DTO
            var carExpenseDto = _mapper.Map<CarExpenseDTO>(createResult.Data);
            #endregion

            return ServiceResult<CarExpenseDTO>.Created(carExpenseDto);
        }
    }
}
