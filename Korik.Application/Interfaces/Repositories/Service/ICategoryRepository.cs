using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<bool> HasUniqueNameAsync(string name, int id);
    }
}
