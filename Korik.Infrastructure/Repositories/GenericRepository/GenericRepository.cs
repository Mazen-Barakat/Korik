using Korik.Application;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Infrastructure
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        #region Dependence Injection

        private readonly Korik _context;

        public GenericRepository(Korik context)
        {
            _context = context;
        }

        #endregion Dependence Injection

        public async Task<T?> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            var numerOfRowAffected = await _context.SaveChangesAsync();
            return numerOfRowAffected > 0 ? entity : null;
        }

        public IQueryable<T>? GetAllAsync()
        {
            ///<summary>
            ///🔎 Why some people return IQueryable<T> for GetAll()?
            ///IQueryable<T> lets the calling code decide:
            ///Filtering(Where)
            ///Sorting(OrderBy)
            ///Pagination(Skip / Take)
            ///Includes(Include, ThenInclude)
            ///The query is not executed yet (deferred execution).
            ///This makes the repo more flexible, but it also means the caller must eventually call .ToListAsync(), .FirstOrDefaultAsync(), etc.to execute it.
            ///</summary>

            return _context.Set<T>().AsNoTracking();
        }

        public IQueryable<T>? GetAllWithIncludeAsync(params Expression<Func<T, object>>[] includes)
        {
            /// <summary>
            /// But calling .AsQueryable() makes it explicit that you’re treating it as a query, not as a list in memory.
            ///Benefit: you can build up LINQ queries dynamically:
            /// </summary>
            var query = _context.Set<T>().AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query.AsNoTracking();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var entity = await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(model => EF.Property<int>(model, "Id") == id);
            return entity!;
        }

        public async Task<T> GetByIdWithIncludeAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            var query = _context.Set<T>().AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            var entity = await query.AsNoTracking().FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
            return entity!;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<T> DeleteAsync(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            return entity;
        }

        public async Task<bool> IsExistAsync(int id)
        {
            var exists = await _context.Set<T>().AnyAsync(e => EF.Property<int>(e, "Id") == id);
            return exists;
        }

        public async Task<PagedResult<T>> GetAllPagedAsync
            (
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            params Expression<Func<T, object>>[]? includes
            )
        {
            var query = _context.Set<T>().AsNoTracking();

            if (includes != null && includes.Length > 0)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            // Apply filter if provided
            if (filter != null)
            {
                query = query.Where(filter);
            }

            var totalRecords = await query.CountAsync();

            var data = await query
                .OrderBy(e => EF.Property<int>(e, "Id"))
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<T>
            {
                Items = data,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }
    }
}