using Korik.Domain;
using Microsoft.EntityFrameworkCore;
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

        public async Task<ServiceResult<IEnumerable<Service>>> GetBySubcategoryIdAsync(int subcategoryId)
        {
            try
            {
                var query = _repository.GetBySubcategoryIdAsync(subcategoryId);
                var list = query != null ? await query.ToListAsync() : new List<Service>();
                return ServiceResult<IEnumerable<Service>>.Ok(list);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<Service>>.Fail(ex.Message);
            }
        }
    }
}
