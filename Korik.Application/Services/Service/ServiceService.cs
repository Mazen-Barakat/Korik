using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class ServiceService :GenericService<Service>, IServiceService
    {
        private readonly IServiceRepository _repository;
        public ServiceService(IServiceRepository repository) : base(repository)
        {
            _repository = repository;
        }

    }
}
