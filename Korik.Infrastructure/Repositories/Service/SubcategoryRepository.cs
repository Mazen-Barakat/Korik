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
    public class SubcategoryRepository : GenericRepository<Subcategory>, ISubcategoryRepository
    {
        private readonly Korik _context;
        public SubcategoryRepository(Korik context) : base(context)
        {
            _context = context;
        }

        public async Task<IQueryable<Subcategory>> GetAllSubcategoriesByCategoryIdAsync(int categoryId)
        {
            var subcategories = await _context.Subcategories
                .Where(sc => sc.CategoryId == categoryId)
                .ToListAsync();

            // Return as IQueryable if required, otherwise consider returning List<Subcategory>
            return subcategories.AsQueryable();
        }
    }
}
