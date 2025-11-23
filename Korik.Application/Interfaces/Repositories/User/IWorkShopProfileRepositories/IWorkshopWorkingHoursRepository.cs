using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public interface IWorkshopWorkingHoursRepository : IGenericRepository<WorkingHours>
    {
        Task<bool> WorkingHourExistsForDayAsync(int workshopId, DayOfWeek day, int excludeId = 0);
        IQueryable<WorkingHours>? GetByWorkshopIdAsync(int workshopId);
    }
}