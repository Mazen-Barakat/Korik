using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using AutoMapper;
using Korik.Domain;

namespace Korik.Application
{
    public record DeleteCarExpenseRequest(DeleteCarExpenseDTO Model) : IRequest<ServiceResult<CarExpenseDTO>>;

    public class DeleteCarExpenseRequestHandler : IRequestHandler<DeleteCarExpenseRequest, ServiceResult<CarExpenseDTO>>
    {
        private readonly ICarExpenseService _service;
        private readonly IValidator<DeleteCarExpenseDTO> _validator;
        private readonly IMapper _mapper;

        public DeleteCarExpenseRequestHandler(
        ICarExpenseService service,
        IValidator<DeleteCarExpenseDTO> validator,
        IMapper mapper)
        {
            _service = service;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<ServiceResult<CarExpenseDTO>> Handle(DeleteCarExpenseRequest request, CancellationToken cancellationToken)
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
            // Perform delete
            var deleteResult = await _service.DeleteAsync(request.Model.Id);

            if (!deleteResult.Success || deleteResult.Data == null)
            {
                return ServiceResult<CarExpenseDTO>.Fail(deleteResult.Message ?? "Failed to delete car expense.");
            }

            // Map deleted entity to DTO
            var carExpenseDto = _mapper.Map<CarExpenseDTO>(deleteResult.Data);
            #endregion

            return ServiceResult<CarExpenseDTO>.Ok(carExpenseDto, "Car expense deleted successfully.");
        }
    }
}
