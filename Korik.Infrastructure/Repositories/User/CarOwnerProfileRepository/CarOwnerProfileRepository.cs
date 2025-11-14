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
    public class CarOwnerProfileRepository : GenericRepository<CarOwnerProfile>, ICarOwnerProfileRepository
    {
        #region Dependence Injection

        private readonly Korik _context;

        public CarOwnerProfileRepository(Korik context) : base(context)
        {
            _context = context;
        }

        #endregion Dependence Injection

        public async Task<CarOwnerProfile?> GetByApplicationUserIdAsync(string applicationUserId)
        {
            var entity = await _context.Set<CarOwnerProfile>().AsNoTracking().FirstOrDefaultAsync(model => EF.Property<string>(model, "ApplicationUserId") == applicationUserId);
            return entity;
        }

        public async Task<CarOwnerProfile> GetByApplicationUserIdWithIncludeAsync(string applicationUserId, params Expression<Func<CarOwnerProfile, object>>[] includes)
        {
            var query = _context.Set<CarOwnerProfile>().AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            var entity = await query.AsNoTracking().FirstOrDefaultAsync(e => EF.Property<string>(e, "ApplicationUserId") == applicationUserId);
            return entity!;
        }
    }
}
