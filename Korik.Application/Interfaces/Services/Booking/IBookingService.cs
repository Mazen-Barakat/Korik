using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public interface IBookingService : IGenericService<Booking>
    {
        Task<ServiceResult<IEnumerable<Booking>>> GetBookingsByCarIdAsync(int carId);

        Task<ServiceResult<IEnumerable<Booking>>> GetBookingsByWorkshopProfileIdAsync(int workshopProfileId);
    }
}
