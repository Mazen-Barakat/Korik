using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public interface ISubcategoryService : IGenericService<Subcategory>
    {
        Task<ServiceResult<IEnumerable<Subcategory>>> GetAllSubcategoriesByCategoryIdAsync(int categoryId);
    }
}
