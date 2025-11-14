using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public interface ICarOwnerProfileService : IGenericService<CarOwnerProfile>
    {
        Task<ServiceResult<CarOwnerProfile>> GetByApplicationUserIdAsync(string applicationUserId);

        Task<ServiceResult<CarOwnerProfile>> GetByApplicationUserIdWithIncludeAsync(string applicationUserId, params Expression<Func<CarOwnerProfile, object>>[] includes);
    }
}
