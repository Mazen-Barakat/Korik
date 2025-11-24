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
    public record UpdateServiceRequest(UpdateServiceDTO model) : IRequest<ServiceResult<ServiceDTO>>{ }

    public class UpdateServiceRequestHandler : IRequestHandler<UpdateServiceRequest, ServiceResult<ServiceDTO>>
    {
        private readonly IServiceService _serviceService;
        private readonly IValidator<UpdateServiceDTO> _validator;
        private readonly IMapper _mapper;

        public UpdateServiceRequestHandler(
            IServiceService serviceService,
            IValidator<UpdateServiceDTO> validator,
            IMapper mapper
            )
        {
            _serviceService = serviceService;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<ServiceResult<ServiceDTO>> Handle(UpdateServiceRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ServiceResult<ServiceDTO>.Fail(string.Join(", ", errors));
            }


            var service = _mapper.Map<Service>(request.model);
            var updatedService = await _serviceService.UpdateAsync(service);
            if (!updatedService.Success)
            {
                return ServiceResult<ServiceDTO>.Fail(updatedService.Message ?? "Failed to update service.");
            }


            var serviceDTO = _mapper.Map<ServiceDTO>(updatedService.Data);
            return ServiceResult<ServiceDTO>.Ok(serviceDTO, "Service updated successfully.");
        }
    }

}
