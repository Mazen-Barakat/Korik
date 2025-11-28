using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class BookingPhotoItemDTO
    {
        public int Id { get; set; }
        public string PhotoUrl { get; set; }
    }

    public class BookingPhotoDTO
    {
        public int BookingId { get; set; }
        public List<BookingPhotoItemDTO> Photos { get; set; } = new();
    }
}