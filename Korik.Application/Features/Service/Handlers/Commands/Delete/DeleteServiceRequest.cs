using AutoMapper;
using Korik.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record DeleteServiceRequest(DeleteServiceDTO model) : IRequest<ServiceResult<ServiceDTO>> { }

    public class DeleteServiceRequestHandler : IRequestHandler<DeleteServiceRequest, ServiceResult<ServiceDTO>>
    {
        private readonly IServiceService _serviceService;
        private readonly IMapper _mapper;

        public DeleteServiceRequestHandler(
            IServiceService serviceService,
            IMapper mapper)
        {
            _serviceService = serviceService;
            _mapper = mapper;
        }

        public async Task<ServiceResult<ServiceDTO>> Handle(DeleteServiceRequest request, CancellationToken cancellationToken)
        {
            var existsResult = await _serviceService.IsExistAsync(request.model.Id);
            if (!existsResult.Data)
            {
                return ServiceResult<ServiceDTO>.Fail("Service not found.");
            }

            var deletedResult = await _serviceService.DeleteAsync(request.model.Id);

            if (!deletedResult.Success)
            {
                return ServiceResult<ServiceDTO>.Fail(deletedResult.Message ?? "Failed to delete service.");
            }

            var serviceDTO = _mapper.Map<ServiceDTO>(deletedResult.Data);
            return ServiceResult<ServiceDTO>.Ok(serviceDTO, "Service deleted successfully.");
        }
    }
}