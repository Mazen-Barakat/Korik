using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public interface IWorkShopProfileService : IGenericService<WorkShopProfile>
    {
        Task<ServiceResult<WorkShopProfile>> GetByApplicationUserIdAsync(string applicationUserId);

        Task<ServiceResult<WorkShopProfile>> GetByApplicationUserIdWithIncludeAsync(string applicationUserId, params Expression<Func<WorkShopProfile, object>>[] includes);

        Task<ServiceResult<PagedResult<WorkShopProfile>>> FilterWorkshopsAsync(PagedRequestDTO pagedRequestDTO);
    }
}