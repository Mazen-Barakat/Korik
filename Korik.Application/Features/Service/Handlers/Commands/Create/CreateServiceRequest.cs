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
    public record CreateServiceRequest(CreateServiceDTO model) : IRequest<ServiceResult<ServiceDTO>> { }


    public class CreateServiceRequestHandler : IRequestHandler<CreateServiceRequest, ServiceResult<ServiceDTO>>
    {
        private readonly IServiceService _serviceService;
        private readonly IValidator<CreateServiceDTO> _validator;
        private readonly IMapper _mapper;
        public CreateServiceRequestHandler
            (
            IServiceService serviceService,
            IValidator<CreateServiceDTO> validator,
            IMapper mapper
            )
        {
            _serviceService = serviceService;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<ServiceResult<ServiceDTO>> Handle(CreateServiceRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ServiceResult<ServiceDTO>.Fail(string.Join(", ", errors));
            }

            var service = _mapper.Map<Service>(request.model);
            var createdServiceResult = await _serviceService.CreateAsync(service);

            if (!createdServiceResult.Success)
            {
                return ServiceResult<ServiceDTO>.Fail(createdServiceResult.Message ?? "Failed to create service.");
            }

            var serviceDTO = _mapper.Map<ServiceDTO>(createdServiceResult.Data);
            return ServiceResult<ServiceDTO>.Created(serviceDTO, "Service created successfully.");
        }
    }
}
