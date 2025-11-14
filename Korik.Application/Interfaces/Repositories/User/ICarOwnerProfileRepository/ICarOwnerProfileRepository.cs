using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public interface ICarOwnerProfileRepository : IGenericRepository<CarOwnerProfile>
    {
        Task<CarOwnerProfile?> GetByApplicationUserIdAsync(string applicationUserId);

        Task<CarOwnerProfile> GetByApplicationUserIdWithIncludeAsync(string applicationUserId, params Expression<Func<CarOwnerProfile, object>>[] includes);
    }
}
