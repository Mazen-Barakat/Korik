using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Korik.Domain;
using AutoMapper;

namespace Korik.Application
{
    public record DeleteCarRequest(DeleteCarDTO Model) : IRequest<ServiceResult<CarDTO>>;

    public class DeleteCarRequestHandler : IRequestHandler<DeleteCarRequest, ServiceResult<CarDTO>>
    {
        private readonly ICarService _service;
        private readonly IValidator<DeleteCarDTO> _validator;
        private readonly IMapper _mapper;


        public DeleteCarRequestHandler
            (ICarService service, 
            IValidator<DeleteCarDTO> validator,
            IMapper mapper
            )
        {
            _service = service;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<ServiceResult<CarDTO>> Handle(DeleteCarRequest request, CancellationToken cancellationToken)
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
            // Perform delete
            var deleteResult = await _service.DeleteAsync(request.Model.Id);

            if (!deleteResult.Success)
            {
                return ServiceResult<CarDTO>.Fail(deleteResult.Message ?? "Failed to delete car.");
            }

            // Map deleted entity to DTO
            var carDto = _mapper.Map<CarDTO>(deleteResult.Data);

            #endregion

            return ServiceResult<CarDTO>.Ok(carDto, "Car deleted successfully.");
        }
    }
}
