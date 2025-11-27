using Korik.Application;
using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Infrastructure
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        private readonly Korik _Context;

        public ReviewRepository(Korik Context) : base(Context)
        {
            _Context = Context;
        }
    }
}
