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
    public record GetWorkShopWorkingHoursByWorkshopIdRequest(GetWorkShopWorkingHoursByWorkshopIdDTO model) : IRequest<ServiceResult<IEnumerable<WorkShopWorkingHoursDTO>>> { }

    public class GetWorkShopWorkingHoursByWorkshopIdRequestHandler : IRequestHandler<GetWorkShopWorkingHoursByWorkshopIdRequest, ServiceResult<IEnumerable<WorkShopWorkingHoursDTO>>>
    {
        private readonly IWorkShopWorkingHoursService _workingHoursService;
        private readonly IMapper _mapper;

        public GetWorkShopWorkingHoursByWorkshopIdRequestHandler(
            IWorkShopWorkingHoursService workingHoursService,
            IMapper mapper)
        {
            _workingHoursService = workingHoursService;
            _mapper = mapper;
        }

        public async Task<ServiceResult<IEnumerable<WorkShopWorkingHoursDTO>>> Handle(GetWorkShopWorkingHoursByWorkshopIdRequest request, CancellationToken cancellationToken)
        {
            var result = await _workingHoursService.GetByWorkshopIdAsync(request.model.WorkShopProfileId);

            if (!result.Success)
            {
                return ServiceResult<IEnumerable<WorkShopWorkingHoursDTO>>.Fail(result.Message ?? "Failed to retrieve working hours.");
            }

            var workingHoursDTOs = _mapper.Map<IEnumerable<WorkShopWorkingHoursDTO>>(result.Data);

            return ServiceResult<IEnumerable<WorkShopWorkingHoursDTO>>.Ok(workingHoursDTOs);
        }
    }
}