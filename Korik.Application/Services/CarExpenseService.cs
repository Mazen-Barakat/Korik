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
    }
}
