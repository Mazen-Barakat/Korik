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
    public record DeleteWorkShopWorkingHoursRequest(DeleteWorkShopWorkingHoursDTO model) : IRequest<ServiceResult<WorkShopWorkingHoursDTO>> { }

    public class DeleteWorkShopWorkingHoursRequestHandler : IRequestHandler<DeleteWorkShopWorkingHoursRequest, ServiceResult<WorkShopWorkingHoursDTO>>
    {
        private readonly IWorkShopWorkingHoursService _workingHoursService;
        private readonly IMapper _mapper;

        public DeleteWorkShopWorkingHoursRequestHandler(
            IWorkShopWorkingHoursService workingHoursService,
            IMapper mapper)
        {
            _workingHoursService = workingHoursService;
            _mapper = mapper;
        }

        public async Task<ServiceResult<WorkShopWorkingHoursDTO>> Handle(DeleteWorkShopWorkingHoursRequest request, CancellationToken cancellationToken)
        {
            var existsResult = await _workingHoursService.IsExistAsync(request.model.Id);
            if (!existsResult.Data)
            {
                return ServiceResult<WorkShopWorkingHoursDTO>.Fail("Working hours not found.");
            }

            var deletedResult = await _workingHoursService.DeleteAsync(request.model.Id);

            if (!deletedResult.Success)
            {
                return ServiceResult<WorkShopWorkingHoursDTO>.Fail(deletedResult.Message ?? "Failed to delete working hours.");
            }

            var workingHoursDTO = _mapper.Map<WorkShopWorkingHoursDTO>(deletedResult.Data);

            return ServiceResult<WorkShopWorkingHoursDTO>.Ok(workingHoursDTO, "Working hours deleted successfully.");
        }
    }
}