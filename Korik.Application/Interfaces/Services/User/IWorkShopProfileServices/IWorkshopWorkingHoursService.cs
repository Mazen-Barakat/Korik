using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public interface IWorkShopWorkingHoursService : IGenericService<WorkingHours>
    {
        Task<ServiceResult<bool>> WorkingHourExistsForDayAsync(int workshopId, DayOfWeek day, int excludeId = 0);
        Task<ServiceResult<IEnumerable<WorkingHours>>> GetByWorkshopIdAsync(int workshopId);
    }
}