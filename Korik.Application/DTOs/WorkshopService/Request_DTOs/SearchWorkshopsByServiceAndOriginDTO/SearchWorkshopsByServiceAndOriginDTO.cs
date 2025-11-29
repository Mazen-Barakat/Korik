using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class SearchWorkshopsByServiceAndOriginDTO
    {
        public int ServiceId { get; set; }

        public CarOrigin? Origin { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}