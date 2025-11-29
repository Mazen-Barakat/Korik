using Korik.Domain;
using Microsoft.EntityFrameworkCore;
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

        public async Task<ServiceResult<IEnumerable<Booking>>> GetBookingsByCarIdAsync(int carId)
        {
            try
            {
                var result = await _bookingRepository.GetBookingsByCarIdAsync(carId).ToListAsync();

                if (result == null || !result.Any())
                {
                    return ServiceResult<IEnumerable<Booking>>.Fail("No bookings found for the specified car ID.");
                }

                return ServiceResult<IEnumerable<Booking>>.Ok(result, "Bookings retrieved successfully.");

            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<Booking>>.Fail($"An error occurred while retrieving bookings: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<Booking>>> GetBookingsByWorkshopProfileIdAsync(int workshopProfileId)
        {
            try
            {
                var result = await _bookingRepository.GetBookingsByWorkshopProfileIdAsync(workshopProfileId).ToListAsync();
                if (result == null || !result.Any())
                {
                    return ServiceResult<IEnumerable<Booking>>.Fail("No bookings found for the specified workshop profile ID.");
                }

                return ServiceResult<IEnumerable<Booking>>.Ok(result, "Bookings retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<Booking>>.Fail($"An error occurred while retrieving bookings: {ex.Message}");
            }
        }
    }
}
