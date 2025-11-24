using Korik.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class WorkShopWorkingHoursService : GenericService<WorkingHours>, IWorkShopWorkingHoursService
    {
        private readonly IWorkshopWorkingHoursRepository _workingHoursRepository;

        public WorkShopWorkingHoursService(IWorkshopWorkingHoursRepository workingHoursRepository)
            : base(workingHoursRepository)
        {
            _workingHoursRepository = workingHoursRepository;
        }

        public async Task<ServiceResult<bool>> WorkingHourExistsForDayAsync(int workshopId, DayOfWeek day, int excludeId = 0)
        {
            try
            {
                var exists = await _workingHoursRepository.WorkingHourExistsForDayAsync(workshopId, day, excludeId);
                return ServiceResult<bool>.Ok(exists);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<IEnumerable<WorkingHours>>> GetByWorkshopIdAsync(int workshopId)
        {
            try
            {
                var query = _workingHoursRepository.GetByWorkshopIdAsync(workshopId);
                var list = query != null ? await query.ToListAsync() : new List<WorkingHours>();
                return ServiceResult<IEnumerable<WorkingHours>>.Ok(list);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkingHours>>.Fail(ex.Message);
            }
        }
    }
}