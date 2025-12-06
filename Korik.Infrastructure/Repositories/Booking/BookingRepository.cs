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
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        private readonly Korik _context;

        public BookingRepository(Korik context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Booking> GetBookingsByCarIdAsync(int carId)
        {
            var result = _context.Bookings.Where(b => b.CarId == carId).AsQueryable();

            return result;
        }

        public IQueryable<Booking> GetBookingsByWorkshopProfileIdAsync(int workshopProfileId)
        {
            var result = _context.Bookings.Where(b => b.WorkShopProfileId == workshopProfileId).AsQueryable();

            return result;
        }

        public IQueryable<BookingServicesWithReviewDTO> GetBookingServicesWithReviewAsync(int carId)
        {
            var result = _context.Bookings
                                .AsNoTracking()
                                .Where(b => b.Status == BookingStatus.Completed && b.CarId == carId)
                                .Select(b => new BookingServicesWithReviewDTO
                                {
                                    Id = b.Id,
                                    AppointmentDate = b.AppointmentDate,
                                    IssueDescription = b.IssueDescription,
                                    ReviewPaidAmount = b.Review != null ? b.Review.PaidAmount : null,
                                    ServiceName = b.WorkshopService.Service.Name,
                                    ServiceDescription = b.WorkshopService.Service.Description
                                });
            return result;
        }
    }
}