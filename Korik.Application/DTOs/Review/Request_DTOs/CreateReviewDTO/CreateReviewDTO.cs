using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class CreateReviewDTO
    {
        public double Rating { get; set; }
        public string Comment { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public int BookingId { get; set; }
        public int CarOwnerProfileId { get; set; }
        public int WorkShopProfileId { get; set; }
    }
}
