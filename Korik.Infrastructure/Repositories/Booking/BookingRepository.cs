using Korik.Application;
using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Infrastructure
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        private readonly Korik _context;
        public BookingRepository(Korik context) : base(context)
        {
            _context = context;
        }
    }
}
