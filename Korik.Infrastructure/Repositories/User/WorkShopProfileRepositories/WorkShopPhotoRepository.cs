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
    public class WorkShopPhotoRepository : GenericRepository<WorkShopPhoto>, IWorkShopPhotoRepository
    {
        private readonly Korik _context;

        public WorkShopPhotoRepository(Korik context) : base(context)
        {
            _context = context;
        }

        public IQueryable<WorkShopPhoto>? GetAllPhotosByWorkShopIdAsync(int WorkShopProfileId)
        {
            return _context.Set<WorkShopPhoto>().Where(w => w.WorkShopProfileId == WorkShopProfileId).AsNoTracking();
        }
    }
}