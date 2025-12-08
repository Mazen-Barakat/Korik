using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record GetAllCarOwnerProfileRequest : IRequest<ServiceResult<IEnumerable<CarOwnerProfileDTO>>>
    {
    }



    public class GetAllCarOwnerProfileRequestHandler : IRequestHandler<GetAllCarOwnerProfileRequest, ServiceResult<IEnumerable<CarOwnerProfileDTO>>>
    {
        private readonly ICarOwnerProfileService _carOwnerProfileService;
        private readonly IMapper _mapper;
        public GetAllCarOwnerProfileRequestHandler(ICarOwnerProfileService carOwnerProfileService, IMapper mapper)
        {
            _carOwnerProfileService = carOwnerProfileService;
            _mapper = mapper;
        }
        public async Task<ServiceResult<IEnumerable<CarOwnerProfileDTO>>> Handle(GetAllCarOwnerProfileRequest request, CancellationToken cancellationToken)
        {
           var carOwnerProfilesResult = await _carOwnerProfileService.GetAllAsync();
            
            if (!carOwnerProfilesResult.Success)
            {
                return ServiceResult<IEnumerable<CarOwnerProfileDTO>>.Fail(carOwnerProfilesResult.Message ?? "Failed to retrieve car owner profiles.");
            }

            var carOwnerProfileDTOs = 
                _mapper.Map<IEnumerable<CarOwnerProfileDTO>>(carOwnerProfilesResult.Data);
            return ServiceResult<IEnumerable<CarOwnerProfileDTO>>.Ok(carOwnerProfileDTOs);
        }
    }
}
