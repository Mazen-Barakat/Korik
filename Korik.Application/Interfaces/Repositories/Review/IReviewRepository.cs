using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<IQueryable<Review>> GetAllReviewsByWorkShopProfileIdAsync(int workerProfileId);

        Task<double> GetAverageRatingsByWorkShopProfileIdAsync(int workerProfileId);
    }
}
