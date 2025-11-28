using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public interface IBookingPhotoService : IGenericService<BookingPhoto>
    {
        Task<ServiceResult<IEnumerable<BookingPhoto>>> GetAllPhotosByBookingIdAsync(int bookingId);
    }
}