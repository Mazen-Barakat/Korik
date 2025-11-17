using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class WorkShopProfileService : GenericService<WorkShopProfile>, IWorkShopProfileService
    {
        private readonly IWorkShopProfileRepository _workShopProfileRepository;

        public WorkShopProfileService(IWorkShopProfileRepository workShopProfileRepository) : base(workShopProfileRepository)
        {
            _workShopProfileRepository = workShopProfileRepository;
        }

        public async Task<ServiceResult<WorkShopProfile>> GetByApplicationUserIdAsync(string applicationUserId)
        {
            try
            {
                var entity = await _workShopProfileRepository.GetByApplicationUserIdAsync(applicationUserId);
                if (entity == null)
                    return ServiceResult<WorkShopProfile>.Fail("profile not found.");

                return ServiceResult<WorkShopProfile>.Ok(entity);
            }
            catch (Exception ex)
            {
                return ServiceResult<WorkShopProfile>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<WorkShopProfile>> GetByApplicationUserIdWithIncludeAsync(string applicationUserId, params Expression<Func<WorkShopProfile, object>>[] includes)
        {
            try
            {
                var entity = await _workShopProfileRepository.GetByApplicationUserIdWithIncludeAsync(applicationUserId, includes);
                if (entity == null)
                    return ServiceResult<WorkShopProfile>.Fail("profile not found.");

                return ServiceResult<WorkShopProfile>.Ok(entity);
            }
            catch (Exception ex)
            {
                return ServiceResult<WorkShopProfile>.Fail(ex.Message);
            }
        }
    }
}