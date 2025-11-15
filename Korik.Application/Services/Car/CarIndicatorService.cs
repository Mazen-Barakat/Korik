using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class CarIndicatorService : GenericService<CarIndicator>, ICarIndicatorService
    {
        private readonly ICarIndicatorRepository _carIndicatorRepository;

        public CarIndicatorService(ICarIndicatorRepository carIndicatorRepository) : base(carIndicatorRepository)
        {
            _carIndicatorRepository = carIndicatorRepository;
        }

        public async Task<ServiceResult<IEnumerable<CarIndicator>>> GetAllCarIndicatorsByCarId(int carId)
        {
            try
            {
                var carIndicators = await _carIndicatorRepository.GetAllCarIndicatorsByCarId(carId);
                return ServiceResult<IEnumerable<CarIndicator>>.Ok(carIndicators);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<CarIndicator>>.Fail($"An error occurred while retrieving car indicators: {ex.Message}");
            }
        }
    }
}
