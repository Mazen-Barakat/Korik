using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application 
{
    public interface IGenericService<T> where T : class
    {
        Task<ServiceResult<IEnumerable<T>>> GetAllAsync();
        Task<ServiceResult<IEnumerable<T>>> GetAllWithIncludeAsync(params Expression<Func<T, object>>[] includes);
        Task<ServiceResult<T>> GetByIdAsync(int id);
        Task<ServiceResult<T>> GetByIdWithIncludeAsync(int id, params Expression<Func<T, object>>[] includes);
        Task<ServiceResult<T>> CreateAsync(T entity);
        Task<ServiceResult<T>> UpdateAsync(T entity);
        Task<ServiceResult<T>> DeleteAsync(int id);
    }
}
