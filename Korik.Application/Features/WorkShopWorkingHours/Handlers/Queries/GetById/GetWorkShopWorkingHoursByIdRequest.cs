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
    public record GetWorkShopWorkingHoursByIdRequest(GetWorkShopWorkingHoursByIdDTO model) : IRequest<ServiceResult<WorkShopWorkingHoursDTO>> { }

    public class GetWorkShopWorkingHoursByIdRequestHandler : IRequestHandler<GetWorkShopWorkingHoursByIdRequest, ServiceResult<WorkShopWorkingHoursDTO>>
    {
        private readonly IWorkShopWorkingHoursService _workingHoursService;
        private readonly IMapper _mapper;

        public GetWorkShopWorkingHoursByIdRequestHandler(
            IWorkShopWorkingHoursService workingHoursService,
            IMapper mapper)
        {
            _workingHoursService = workingHoursService;
            _mapper = mapper;
        }

        public async Task<ServiceResult<WorkShopWorkingHoursDTO>> Handle(GetWorkShopWorkingHoursByIdRequest request, CancellationToken cancellationToken)
        {
            var result = await _workingHoursService.GetByIdAsync(request.model.Id);

            if (!result.Success || result.Data == null)
            {
                return ServiceResult<WorkShopWorkingHoursDTO>.Fail("Working hours not found.");
            }

            var workingHoursDTO = _mapper.Map<WorkShopWorkingHoursDTO>(result.Data);

            return ServiceResult<WorkShopWorkingHoursDTO>.Ok(workingHoursDTO);
        }
    }
}