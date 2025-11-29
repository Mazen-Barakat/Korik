using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        IQueryable<Booking> GetBookingsByCarIdAsync(int carId);
        IQueryable<Booking> GetBookingsByWorkshopProfileIdAsync(int workshopProfileId);
    }
}
