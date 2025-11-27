using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Domain
{
    public class Review : BaseEntity
    {
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public decimal? PaidAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        #region Booking 1---1 Review
        [ForeignKey("Booking")]
        public int BookingId { get; set; }
        public Booking Booking { get; set; }
        #endregion

        #region Review M---1 WorkShopProfile
        [ForeignKey("WorkShopProfile")]
        public int WorkShopProfileId { get; set; }
        public WorkShopProfile WorkShopProfile { get; set; }
        #endregion

        #region Review M---1 CarOwnerProfile
        [ForeignKey("CarOwnerProfile")]
        public int CarOwnerProfileId { get; set; }
        public CarOwnerProfile CarOwnerProfile { get; set; }
        #endregion
    }
}
