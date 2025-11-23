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
    public record GetAllServicesRequest : IRequest<ServiceResult<IEnumerable<ServiceDTO>>> { }

    public class GetAllServicesRequestHandler : IRequestHandler<GetAllServicesRequest, ServiceResult<IEnumerable<ServiceDTO>>>
    {
        private readonly IServiceService _serviceService;
        private readonly IMapper _mapper;

        public GetAllServicesRequestHandler(
            IServiceService serviceService,
            IMapper mapper)
        {
            _serviceService = serviceService;
            _mapper = mapper;
        }

        public async Task<ServiceResult<IEnumerable<ServiceDTO>>> Handle(GetAllServicesRequest request, CancellationToken cancellationToken)
        {
            // Get all services with Subcategory included
            var result = await _serviceService.GetAllWithIncludeAsync(s => s.Subcategory);

            if (!result.Success)
            {
                return ServiceResult<IEnumerable<ServiceDTO>>.Fail(result.Message ?? "Failed to retrieve services.");
            }

            var serviceDTOs = _mapper.Map<IEnumerable<ServiceDTO>>(result.Data);
            return ServiceResult<IEnumerable<ServiceDTO>>.Ok(serviceDTOs);
        }
    }
}