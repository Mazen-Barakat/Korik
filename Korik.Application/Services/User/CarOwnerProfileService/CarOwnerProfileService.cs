using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class CarOwnerProfileService : GenericService<CarOwnerProfile>, ICarOwnerProfileService
    {
        private readonly ICarOwnerProfileRepository _repository;

        public CarOwnerProfileService(ICarOwnerProfileRepository repository) : base(repository)
        {
            _repository = repository;
        }
    }
}
