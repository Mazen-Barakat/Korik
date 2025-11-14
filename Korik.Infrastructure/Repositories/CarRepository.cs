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
    public class CarRepository : GenericRepository<Car>, ICarRepository
    {
        private readonly Korik _context;

        public CarRepository(Korik Context) : base(Context)
        {
            _context = Context;
        }

        public async Task<IQueryable<Car>> GetAllCarsByCarOwnerProfileIdAsync(int carOwnerProfileId)
        {
            var cars = _context.Cars.Where(c => c.CarOwnerProfileId == carOwnerProfileId);
            return cars;
        }

        public async Task<Car> GetByLicensePlateAsync(string licensePlate)
        {
            var entity = await _context.Cars
                .FirstOrDefaultAsync(c => c.LicensePlate == licensePlate);
            return entity!;
        }

    }
}
