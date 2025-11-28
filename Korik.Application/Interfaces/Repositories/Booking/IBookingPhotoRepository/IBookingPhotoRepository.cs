using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public interface IBookingPhotoRepository : IGenericRepository<BookingPhoto>
    {
        IQueryable<BookingPhoto>? GetAllPhotosByBookingId(int bookingId);
    }
}