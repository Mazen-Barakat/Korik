using Korik.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public interface ICarRepository : IGenericRepository<Car>
    {
        Task<bool> GetByLicensePlateAsync(string licensePlate, int id);

        Task<IQueryable<Car>> GetAllCarsByCarOwnerProfileIdAsync(int carOwnerProfileId);
    }
}
