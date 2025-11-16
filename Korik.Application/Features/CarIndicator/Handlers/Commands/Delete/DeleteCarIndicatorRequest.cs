using AutoMapper;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record DeleteCarIndicatorRequest(DeleteCarIndicatorDTO Model) : IRequest<ServiceResult<CarIndicatorDTO>>;

    public class DeleteCarIndicatorRequestHandler : IRequestHandler<DeleteCarIndicatorRequest, ServiceResult<CarIndicatorDTO>>
    {
        private readonly ICarIndicatorService _carIndicatorService;
        private readonly IValidator<DeleteCarIndicatorDTO> _validator;
        private readonly IMapper _mapper;

        public DeleteCarIndicatorRequestHandler(
            ICarIndicatorService carIndicatorService,
            IValidator<DeleteCarIndicatorDTO> validator,
            IMapper mapper)
        {
            _carIndicatorService = carIndicatorService;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<ServiceResult<CarIndicatorDTO>> Handle(DeleteCarIndicatorRequest request, CancellationToken cancellationToken)
        {
            #region Not Valid
            // Validate the request
            var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<CarIndicatorDTO>.Fail(errors);
            } 
            #endregion


            // Call the service to delete the car indicator
            var result = await _carIndicatorService.DeleteAsync(request.Model.Id);
            if (!result.Success)
            {
                return ServiceResult<CarIndicatorDTO>.Fail(result.Message ?? "Unable to delete car indicator.");
            }

            return ServiceResult<CarIndicatorDTO>.Ok(_mapper.Map<CarIndicatorDTO>(result.Data), "Car indicator deleted successfully.");
        }
    }
}
