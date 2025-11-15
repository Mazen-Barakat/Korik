using Korik.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public interface ICarService : IGenericService<Car>
    {
        Task<ServiceResult<bool>> GetByLicensePlateAsync(string licensePlate, int id);

        Task<ServiceResult<IEnumerable<Car>>> GetAllCarsByCarOwnerProfileIdAsync(int carOwnerProfileId);
    }
}
