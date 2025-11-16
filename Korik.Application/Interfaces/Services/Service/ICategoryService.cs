using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public interface ICategoryService : IGenericService<Category>
    {
        Task<ServiceResult<bool>> HasUniqueNameAsync(string name, int id);
    }
}
