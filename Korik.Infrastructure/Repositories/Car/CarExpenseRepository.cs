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

        public Task<IQueryable<CarExpenses>> GetAllCarExpensesByCarId(int carId)
        {
            var carExpenses = _context.CarExpenses.Where(ce => ce.CarId == carId);
            return Task.FromResult(carExpenses);
        }
    }
}
