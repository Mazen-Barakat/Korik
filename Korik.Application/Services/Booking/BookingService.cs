using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class BookingService : GenericService<Booking>, IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        public BookingService(IBookingRepository bookingRepository) : base(bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }
    }
}
