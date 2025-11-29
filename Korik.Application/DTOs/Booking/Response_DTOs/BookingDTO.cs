using Korik.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class BookingDTO
    {
        public int Id { get; set; }
        public BookingStatus Status { get; set; } 
        public DateTime AppointmentDate { get; set; }
        public string IssueDescription { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal? PaidAmount { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime CreatedAt { get; set; }

        public int CarId { get; set; }
        public int WorkShopProfileId { get; set; }
        public int WorkshopServiceId { get; set; }
  
    }
}
