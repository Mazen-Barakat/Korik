using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Domain
{
    public class Service : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        #region Service M----1 Subcategory

        [ForeignKey("Subcategory")]
        public int SubcategoryId { get; set; }

        public Subcategory Subcategory { get; set; }

        #endregion Service M----1 Subcategory

        #region Service 1---M Booking

        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        #endregion Service 1---M Booking

        #region Service 1---M WorkshopService

        public virtual ICollection<WorkshopService> WorkshopServices { get; set; } = new List<WorkshopService>();

        #endregion Service 1---M WorkshopService
    }
}