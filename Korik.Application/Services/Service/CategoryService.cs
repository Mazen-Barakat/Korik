using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class CategoryService : GenericService<Category>, ICategoryService
    {
        private readonly ICategoryRepository _repository;
        public CategoryService(ICategoryRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResult<bool>> HasUniqueNameAsync(string name , int id)
        {
            try
            {
                var result = await _repository.HasUniqueNameAsync(name , id);

                if (result)
                {
                    return ServiceResult<bool>.Ok(true);
                }
                return ServiceResult<bool>.Fail("Category name is already in use.");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
            }
        }
    }
}
