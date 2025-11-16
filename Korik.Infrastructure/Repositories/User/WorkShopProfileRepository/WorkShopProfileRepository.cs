using Korik.Application;
using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
