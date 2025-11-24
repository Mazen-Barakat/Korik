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
    public class ServiceRepository : GenericRepository<Service>, IServiceRepository
    {
        private readonly Korik _context;
        public ServiceRepository(Korik context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Service>? GetBySubcategoryIdAsync(int subcategoryId)
        {
            return _context.Services
                .Where(s => s.SubcategoryId == subcategoryId)
                .AsNoTracking();
        }
    }
}
