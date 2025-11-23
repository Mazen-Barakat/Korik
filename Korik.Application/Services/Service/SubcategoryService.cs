using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class SubcategoryService : GenericService<Subcategory>, ISubcategoryService
    {
        private readonly ISubcategoryRepository _repository;
        public SubcategoryService(ISubcategoryRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResult<IEnumerable<Subcategory>>> GetAllSubcategoriesByCategoryIdAsync(int categoryId)
        {
            try
            {
                var subcategories =  await _repository.GetAllSubcategoriesByCategoryIdAsync(categoryId);

                if (subcategories == null || !subcategories.Any())
                {
                    return ServiceResult<IEnumerable<Subcategory>>.Fail("No subcategories found for the given category ID.");
                }
                return ServiceResult<IEnumerable<Subcategory>>.Ok(subcategories);
            }

            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<Subcategory>>.Fail(ex.Message);
            }
        }
    }
}
