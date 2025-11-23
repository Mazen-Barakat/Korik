using Korik.Application;
using Korik.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Infrastructure
{
    public class WorkshopWorkingHoursRepository : GenericRepository<WorkingHours>, IWorkshopWorkingHoursRepository
    {
        private readonly Korik _context;

        public WorkshopWorkingHoursRepository(Korik context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> WorkingHourExistsForDayAsync(int workshopId, DayOfWeek day, int excludeId = 0)
        {
            return await _context.WorkingHours
                .AnyAsync(wh => wh.WorkShopProfileId == workshopId
                    && wh.Day == day
                    && wh.Id != excludeId);
        }

        public IQueryable<WorkingHours>? GetByWorkshopIdAsync(int workshopId)
        {
            return _context.WorkingHours
                .Where(wh => wh.WorkShopProfileId == workshopId)
                .AsNoTracking();
        }
    }
}