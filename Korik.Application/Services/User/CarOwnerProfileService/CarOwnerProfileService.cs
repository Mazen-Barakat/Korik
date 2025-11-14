using Korik.Application;
using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class CarOwnerProfileService : GenericService<CarOwnerProfile>, ICarOwnerProfileService
    {
        #region Dependence Injection

        private readonly ICarOwnerProfileRepository _repository;

        public CarOwnerProfileService(ICarOwnerProfileRepository repository) : base(repository)
        {
            _repository = repository;
        }

        #endregion Dependence Injection

        public async Task<ServiceResult<CarOwnerProfile>> GetByApplicationUserIdAsync(string applicationUserId)
        {
            try
            {
                var entity = await _repository.GetByApplicationUserIdAsync(applicationUserId);
                if (entity == null)
                    return ServiceResult<CarOwnerProfile>.Fail("profile not found.");

                return ServiceResult<CarOwnerProfile>.Ok(entity);
            }
            catch (Exception ex)
            {
                return ServiceResult<CarOwnerProfile>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<CarOwnerProfile>> GetByApplicationUserIdWithIncludeAsync(string applicationUserId, params Expression<Func<CarOwnerProfile, object>>[] includes)
        {
            try
            {
                var entity = await _repository.GetByApplicationUserIdWithIncludeAsync(applicationUserId, includes);
                if (entity == null)
                    return ServiceResult<CarOwnerProfile>.Fail("profile not found.");

                return ServiceResult<CarOwnerProfile>.Ok(entity);
            }
            catch (Exception ex)
            {
                return ServiceResult<CarOwnerProfile>.Fail(ex.Message);
            }
        }
    }
}
