using Korik.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class CarService : GenericService<Car>, ICarService
    {
        private readonly ICarRepository _repository;

        public CarService(ICarRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResult<Car>> GetByLicensePlateAsync(string licensePlate)
        {
            try
            {
                var entity = await _repository.GetByLicensePlateAsync(licensePlate);
                if (entity == null)
                    return ServiceResult<Car>.Fail("Entity not found.");

                return ServiceResult<Car>.Ok(entity);
            }
            catch (Exception ex)
            {
                return ServiceResult<Car>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<IEnumerable<Car>>> GetAllCarsByCarOwnerProfileIdAsync(int carOwnerProfileId)
        {
            try
            {
                var entities = await _repository.GetAllCarsByCarOwnerProfileIdAsync(carOwnerProfileId);
                return ServiceResult<IEnumerable<Car>>.Ok(entities);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<Car>>.Fail(ex.Message);
            }
        }
    }
}
