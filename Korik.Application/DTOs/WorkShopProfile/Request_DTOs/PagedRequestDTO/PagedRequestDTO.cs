using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class PagedRequestDTO
    {
        public string? Name { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public string? Country { get; set; }
        public string? Governorate { get; set; }
        public string? City { get; set; }

        public bool? DESCRating { get; set; }

        public WorkShopType? WorkShopType { get; set; }

        public CarOrigin? Origin { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}