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



    }
}
