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
    public class BookingPhotoRepository : GenericRepository<BookingPhoto>, IBookingPhotoRepository
    {
        private readonly Korik _context;

        public BookingPhotoRepository(Korik context) : base(context)
        {
            _context = context;
        }

        public IQueryable<BookingPhoto>? GetAllPhotosByBookingId(int bookingId)
        {
            return _context.Set<BookingPhoto>().Where(p => p.BookingId == bookingId).AsNoTracking();
        }
    }
}