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
    public record GetServiceByIdRequest(GetServiceByIdDTO model) : IRequest<ServiceResult<ServiceDTO>> { }

    public class GetServiceByIdRequestHandler : IRequestHandler<GetServiceByIdRequest, ServiceResult<ServiceDTO>>
    {
        private readonly IServiceService _serviceService;
        private readonly IMapper _mapper;

        public GetServiceByIdRequestHandler(
            IServiceService serviceService,
            IMapper mapper)
        {
            _serviceService = serviceService;
            _mapper = mapper;
        }

        public async Task<ServiceResult<ServiceDTO>> Handle(GetServiceByIdRequest request, CancellationToken cancellationToken)
        {
            // Get service with Subcategory included
            var result = await _serviceService.GetByIdWithIncludeAsync(
                request.model.Id,
                s => s.Subcategory
            );

            if (!result.Success || result.Data == null)
            {
                return ServiceResult<ServiceDTO>.Fail("Service not found.");
            }

            var serviceDTO = _mapper.Map<ServiceDTO>(result.Data);
            return ServiceResult<ServiceDTO>.Ok(serviceDTO);
        }
    }
}