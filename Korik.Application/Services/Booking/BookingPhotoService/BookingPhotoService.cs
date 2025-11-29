using Korik.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class BookingPhotoService : GenericService<BookingPhoto>, IBookingPhotoService
    {
        private readonly IBookingPhotoRepository _repository;

        public BookingPhotoService(IBookingPhotoRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResult<IEnumerable<BookingPhoto>>> GetAllPhotosByBookingIdAsync(int bookingId)
        {
            try
            {
                var query = _repository.GetAllPhotosByBookingId(bookingId);
                var list = query != null ? await query.ToListAsync() : new List<BookingPhoto>();
                return ServiceResult<IEnumerable<BookingPhoto>>.Ok(list);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<BookingPhoto>>.Fail(ex.Message);
            }
        }
    }
}