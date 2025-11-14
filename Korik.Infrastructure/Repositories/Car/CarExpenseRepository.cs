using Korik.Application;
using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Infrastructure
{
    public class CarExpenseRepository : GenericRepository<CarExpenses>, ICarExpenseRepository
    {
        private readonly Korik _context;

        public CarExpenseRepository(Korik Context) : base(Context)
        {
            _context = Context;
        }
    }
}
