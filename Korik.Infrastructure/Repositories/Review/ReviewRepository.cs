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

        public IQueryable<Review> GetAllReviewsByWorkShopProfileIdAsync(int workerProfileId)
        {
            return _context.Reviews
                .Where(r => r.WorkShopProfileId == workerProfileId)
                .Include(r => r.CarOwnerProfile)
                .AsNoTracking();
        }

        public async Task<double> GetAverageRatingsByWorkShopProfileIdAsync(int workerProfileId)
        {
            var rating = await _context.Reviews.Where(r => r.WorkShopProfileId == workerProfileId)
                .AverageAsync(r => (double?)r.Rating) ?? 0.0;

            return rating;
        }
    }
}