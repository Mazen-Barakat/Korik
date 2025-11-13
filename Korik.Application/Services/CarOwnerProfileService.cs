using Korik.Application;
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
        private readonly IGenericRepository<CarOwnerProfile> _repository;

        public CarOwnerProfileService(IGenericRepository<CarOwnerProfile> repository) : base(repository)
        {
            _repository = repository;
        }
    }
}