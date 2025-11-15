using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T>? GetAllAsync();
        IQueryable<T>? GetAllWithIncludeAsync(params Expression<Func<T, object>>[] includes);
        Task<T> GetByIdAsync(int id);
        Task<T> GetByIdWithIncludeAsync(int id, params Expression<Func<T, object>>[] includes);
        Task<T?> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(int id);
        Task<bool> IsExistAsync(int id);
    }
}
