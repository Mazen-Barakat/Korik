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

        public async Task<ServiceResult<PagedResult<WorkShopProfile>>> FilterWorkshopsAsync(PagedRequestDTO pagedRequestDTO)
        {
            try
            {
                var pagedResult = await _workShopProfileRepository.FilterWorkshopsAsync
                    (
                    PageNumber: pagedRequestDTO.PageNumber,
                    PageSize: pagedRequestDTO.PageSize,
                    Name: pagedRequestDTO.Name,
                    Latitude: pagedRequestDTO.Latitude,
                    Longitude: pagedRequestDTO.Longitude,
                    Country: pagedRequestDTO.Country,
                    Governorate: pagedRequestDTO.Governorate,
                    City: pagedRequestDTO.City,
                    DESCRating: pagedRequestDTO.DESCRating,
                    WorkShopType: pagedRequestDTO.WorkShopType,
                    Origin: pagedRequestDTO.Origin
                    );

                if (pagedResult == null)
                    return ServiceResult<PagedResult<WorkShopProfile>>.Fail("profile not found.");

                return ServiceResult<PagedResult<WorkShopProfile>>.Ok(pagedResult);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<WorkShopProfile>>.Fail(ex.Message);
            }
        }
    }
}