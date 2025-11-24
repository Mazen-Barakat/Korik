using Korik.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class WorkShopPhotoService : GenericService<WorkShopPhoto>, IWorkShopPhotoService
    {
        private readonly IWorkShopPhotoRepository _repository;

        public WorkShopPhotoService(IWorkShopPhotoRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResult<IEnumerable<WorkShopPhoto>>> GetAllPhotosByWorkShopIdAsync(int WorkShopProfileId)
        {
            try
            {
                var query = _repository.GetAllPhotosByWorkShopIdAsync(WorkShopProfileId);
                var list = query != null ? await query.ToListAsync() : new List<WorkShopPhoto>();
                return ServiceResult<IEnumerable<WorkShopPhoto>>.Ok(list);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<WorkShopPhoto>>.Fail(ex.Message);
            }
        }
    }
}