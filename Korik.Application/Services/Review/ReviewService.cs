using Korik.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class ReviewService : GenericService<Review>, IReviewService
    {
        private readonly IReviewRepository _repository;

        public ReviewService(IReviewRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResult<IEnumerable<Review>>> GetAllReviewsByWorkShopProfileIdAsync(int workerProfileId)
        {
            try
            {
                var reviewsQueryable = _repository.GetAllReviewsByWorkShopProfileIdAsync(workerProfileId);
                var reviews = await reviewsQueryable.ToListAsync();
                return ServiceResult<IEnumerable<Review>>.Ok(reviews);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<Review>>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<double>> GetAverageRatingsByWorkShopProfileIdAsync(int workerProfileId)
        {
            try
            {
                var averageRating = await _repository.GetAverageRatingsByWorkShopProfileIdAsync(workerProfileId);
                return ServiceResult<double>.Ok(averageRating);
            }
            catch (Exception ex)
            {
                return ServiceResult<double>.Fail(ex.Message);
            }
        }
    }
}