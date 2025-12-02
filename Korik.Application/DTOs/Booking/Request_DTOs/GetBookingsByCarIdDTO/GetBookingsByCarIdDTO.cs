using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class GetBookingsByCarIdDTO
    {
        public int CarId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
