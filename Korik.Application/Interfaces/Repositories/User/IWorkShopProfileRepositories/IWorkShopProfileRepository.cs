using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public interface IWorkShopProfileRepository : IGenericRepository<WorkShopProfile>
    {
        Task<WorkShopProfile?> GetByApplicationUserIdAsync(string applicationUserId);

        Task<WorkShopProfile?> GetByApplicationUserIdWithIncludeAsync(string applicationUserId, params Expression<Func<WorkShopProfile, object>>[] includes);
    }
}