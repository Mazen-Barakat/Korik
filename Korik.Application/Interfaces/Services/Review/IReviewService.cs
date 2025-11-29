using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public interface IReviewService : IGenericService<Review>
    {
        Task<ServiceResult<double>> GetAverageRatingsByWorkShopProfileIdAsync(int workerProfileId);

        Task<ServiceResult<IEnumerable<Review>>> GetAllReviewsByWorkShopProfileIdAsync(int workerProfileId);

        //Task<ServiceResult<List<Review>>> GetReviewsByUserIdAsync(int userId);  
    }
}
