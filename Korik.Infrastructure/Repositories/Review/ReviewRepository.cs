using Korik.Application;
using Korik.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Infrastructure
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        private readonly Korik _context;

        public ReviewRepository(Korik context) : base(context)
        {
            _context = context;
        }

        public async Task<IQueryable<Review>> GetAllReviewsByWorkShopProfileIdAsync(int workerProfileId)
        {
            return _context.Reviews.Where(r => r.WorkShopProfileId == workerProfileId).AsNoTracking();
        }

        public async Task<double> GetAverageRatingsByWorkShopProfileIdAsync(int workerProfileId)
        {
            var reviews = _context.Reviews.Where(r => r.WorkShopProfileId == workerProfileId);
            if (!reviews.Any())
            {
                return 0.0;
            }
            return reviews.Average(r => r.Rating);
        }
    }
}
