using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class BookingServicesWithReviewDTO
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string IssueDescription { get; set; }

        //Review
        public decimal? ReviewPaidAmount { get; set; }

        //service
        public string ServiceName { get; set; }

        public string? ServiceDescription { get; set; }
    }
}