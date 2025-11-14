using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Korik.Domain;

namespace Korik.Application
{
    public record GetByIdCarExpenseRequest(GetByIdCarExpenseDTO Model) : IRequest<ServiceResult<CarExpenseDTO>>;

    public class GetByIdCarExpenseRequestHandler : IRequestHandler<GetByIdCarExpenseRequest, ServiceResult<CarExpenseDTO>>
    {
        private readonly ICarExpenseService _service;
        private readonly IValidator<GetByIdCarExpenseDTO> _validator;
        private readonly IMapper _mapper;

        public GetByIdCarExpenseRequestHandler(
            ICarExpenseService service,
            IValidator<GetByIdCarExpenseDTO> validator,
            IMapper mapper)
        {
            _service = service;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<ServiceResult<CarExpenseDTO>> Handle(GetByIdCarExpenseRequest request, CancellationToken cancellationToken)
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
            // Retrieve car expense by Id
            var expenseResult = await _service.GetByIdAsync(request.Model.Id);

            if (!expenseResult.Success || expenseResult.Data == null)
            {
                return ServiceResult<CarExpenseDTO>.Fail("Car expense not found.");
            }

            // Map to DTO
            var carExpenseDto = _mapper.Map<CarExpenseDTO>(expenseResult.Data);
            #endregion

            return ServiceResult<CarExpenseDTO>.Ok(carExpenseDto);
        }
    }
}
