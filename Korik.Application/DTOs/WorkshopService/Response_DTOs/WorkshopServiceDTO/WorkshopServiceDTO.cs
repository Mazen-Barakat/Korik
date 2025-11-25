using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class WorkshopServiceDTO
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public int WorkShopProfileId { get; set; }
        public int Duration { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public CarOrigin Origin { get; set; }
    }
}