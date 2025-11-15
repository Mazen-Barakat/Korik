using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class CarExpenseService : GenericService<CarExpenses>, ICarExpenseService
    {
        private readonly ICarExpenseRepository _repository;

        public CarExpenseService(ICarExpenseRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResult<IEnumerable<CarExpenses>>> GetAllCarExpensesByCarId(int carId)
        {
            try
            {
                var expenses = await _repository.GetAllCarExpensesByCarId(carId);

                return ServiceResult<IEnumerable<CarExpenses>>.Ok(expenses);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<CarExpenses>>.Fail(ex.Message);
            }
        }
    }
}
