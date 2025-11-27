using Korik.Domain;
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
    }
}
