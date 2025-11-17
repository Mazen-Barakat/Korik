using Korik.Application;
using Korik.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Infrastructure
{
    public class WorkShopProfileRepository : GenericRepository<WorkShopProfile>, IWorkShopProfileRepository
    {
        private readonly Korik _context;

        public WorkShopProfileRepository(Korik context) : base(context)
        {
            _context = context;
        }

        public async Task<WorkShopProfile?> GetByApplicationUserIdAsync(string applicationUserId)
        {
            var entity = await _context.Set<WorkShopProfile>().AsNoTracking().FirstOrDefaultAsync(e => e.ApplicationUserId == applicationUserId);
            return entity;
        }

        public async Task<WorkShopProfile?> GetByApplicationUserIdWithIncludeAsync(string applicationUserId, params Expression<Func<WorkShopProfile, object>>[] includes)
        {
            var query = _context.Set<WorkShopProfile>().AsQueryable();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            var entity = await query.AsNoTracking().FirstOrDefaultAsync(e => e.ApplicationUserId == applicationUserId);
            return entity;
        }
    }
}