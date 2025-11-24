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
    public record GetServicesBySubcategoryIdRequest(GetServicesBySubcategoryIdDTO model) : IRequest<ServiceResult<IEnumerable<ServiceDTO>>> { }

    public class GetServicesBySubcategoryIdRequestHandler : IRequestHandler<GetServicesBySubcategoryIdRequest, ServiceResult<IEnumerable<ServiceDTO>>>
    {
        private readonly IServiceService _serviceService;
        private readonly IMapper _mapper;

        public GetServicesBySubcategoryIdRequestHandler(
            IServiceService serviceService,
            IMapper mapper)
        {
            _serviceService = serviceService;
            _mapper = mapper;
        }

        public async Task<ServiceResult<IEnumerable<ServiceDTO>>> Handle(GetServicesBySubcategoryIdRequest request, CancellationToken cancellationToken)
        {
            var result = await _serviceService.GetBySubcategoryIdAsync(request.model.SubcategoryId);

            if (!result.Success)
            {
                return ServiceResult<IEnumerable<ServiceDTO>>.Fail(result.Message ?? "Failed to retrieve services.");
            }

            var serviceDTOs = _mapper.Map<IEnumerable<ServiceDTO>>(result.Data);
            return ServiceResult<IEnumerable<ServiceDTO>>.Ok(serviceDTOs);
        }
    }
}