using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class ReviewWithProfileDTO
    {
        public double Rating { get; set; }
        public string Comment { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime CreatedAt { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}