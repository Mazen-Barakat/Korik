using Korik.Application;
using Korik.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Korik.Infrastructure
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly Korik _context;
        public CategoryRepository(Korik context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> HasUniqueNameAsync(string name, int id = 0)
        {
            return await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == name.ToLower() && (c.Id != id));
        }

    }
}
