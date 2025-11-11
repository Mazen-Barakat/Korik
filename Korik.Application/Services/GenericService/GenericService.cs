using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{ 
    public class GenericService<T> : IGenericService<T> where T : class
    {
        private readonly IGenericRepository<T> _repository;

        public GenericService(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResult<IEnumerable<T>>> GetAllAsync()
        {
            try
            {
                var query = _repository.GetAllAsync();
                var list = query != null ? await query.ToListAsync() : new List<T>();
                return ServiceResult<IEnumerable<T>>.Ok(list);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<T>>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<IEnumerable<T>>> GetAllWithIncludeAsync(params Expression<Func<T, object>>[] includes)
        {
            try
            {
                var query = _repository.GetAllWithIncludeAsync(includes);
                var list = query != null ? await query.ToListAsync() : new List<T>();
                return ServiceResult<IEnumerable<T>>.Ok(list);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<T>>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<T>> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                    return ServiceResult<T>.Fail("Entity not found.");

                return ServiceResult<T>.Ok(entity);
            }
            catch (Exception ex)
            {
                return ServiceResult<T>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<T>> GetByIdWithIncludeAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            try
            {
                var entity = await _repository.GetByIdWithIncludeAsync(id, includes);
                if (entity == null)
                    return ServiceResult<T>.Fail("Entity not found.");

                return ServiceResult<T>.Ok(entity);
            }
            catch (Exception ex)
            {
                return ServiceResult<T>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<T>> CreateAsync(T entity)
        {
            try
            {
                var created = await _repository.AddAsync(entity);
                if (created == null)
                    return ServiceResult<T>.Fail("Failed to create entity.");

                return ServiceResult<T>.Ok(created);
            }
            catch (Exception ex)
            {
                return ServiceResult<T>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<T>> UpdateAsync(T entity)
        {
            try
            {
                var updated = await _repository.UpdateAsync(entity);
                return ServiceResult<T>.Ok(updated);
            }
            catch (Exception ex)
            {
                return ServiceResult<T>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<T>> DeleteAsync(int id)
        {
            try
            {
                var deleted = await _repository.DeleteAsync(id);
                if (deleted == null)
                    return ServiceResult<T>.Fail("Entity not found.");

                return ServiceResult<T>.Ok(deleted);
            }
            catch (Exception ex)
            {
                return ServiceResult<T>.Fail(ex.Message);
            }
        }
    }
}
