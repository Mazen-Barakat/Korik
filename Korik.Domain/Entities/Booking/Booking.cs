using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Domain
{
    public enum BookingStatus
    {
        Pending,
        Confirmed,
        InProgress,
        Cancelled,
        ReadyForPickup,
        Completed,
        Rejected,
        NoShow
    }

    public enum PaymentMethod
    {
        Cash,
        CreditCard,
    }

    public enum PaymentStatus
    {
        Unpaid,
        Paid,
    }


    public class Booking : BaseEntity
    {
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public DateTime AppointmentDate { get; set; }
        public string IssueDescription { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal? PaidAmount { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        #region Booking 1---M BookingPhoto
        public virtual ICollection<BookingPhoto> BookingPhotos { get; set; } = new List<BookingPhoto>();
        #endregion


        #region Booking M---1 Car
        [ForeignKey("Car")]
        public int CarId { get; set; }
        public virtual Car Car { get; set; }
        #endregion


        #region Booking M---1 WorkShopProfile
        [ForeignKey("WorkShopProfile")]
        public int WorkShopProfileId { get; set; }
        public virtual WorkShopProfile WorkShopProfile { get; set; }
        #endregion


        #region Booking 1---1 Review
        public virtual Review? Review { get; set; }
        #endregion


        #region Booking M---1 WorkshopService
        [ForeignKey("WorkshopService")]
        public int WorkshopServiceId { get; set; }
        public virtual WorkshopService WorkshopService { get; set; }

        #endregion


    }
}
