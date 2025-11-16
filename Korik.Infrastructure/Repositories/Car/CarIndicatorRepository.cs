using Korik.Application;
using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Infrastructure
{
    public class CarIndicatorRepository : GenericRepository<CarIndicator>, ICarIndicatorRepository
    {
        private readonly Korik _context;

        public CarIndicatorRepository(Korik Context) : base(Context)
        {
            _context = Context;
        }

        public Task<IQueryable<CarIndicator>> GetAllCarIndicatorsByCarId(int CarId)
        {
            var result = _context.CarIndicators.Where(ci => ci.CarId == CarId).AsQueryable();
            return Task.FromResult(result);
        }
    }
}
