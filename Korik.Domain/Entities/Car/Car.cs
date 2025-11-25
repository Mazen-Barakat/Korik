using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Domain
{
    public enum TransmissionType
    {
        Manual,
        Automatic,
        SemiAutomatic
    }

    public enum FuelType
    {
        Gasoline,
        CNG,
    }

    public class Car : BaseEntity
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public int EngineCapacity { get; set; }
        public int CurrentMileage { get; set; }
        public string LicensePlate { get; set; }
        public TransmissionType TransmissionType { get; set; }
        public FuelType FuelType { get; set; }

        public CarOrigin Origin { get; set; }


        #region Car M----1 CarOwnerProfile
        public virtual CarOwnerProfile CarOwnerProfile { get; set; }
        [ForeignKey("CarOwnerProfile")]
        public int CarOwnerProfileId { get; set; }
        #endregion

        #region Car 1---M Booking
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        #endregion

        #region Car 1---M CarExpenses
        public ICollection<CarExpenses> CarExpenses { get; set; } = new List<CarExpenses>();
        #endregion

        #region Car 1---M CarIdicator
        public ICollection<CarIndicator> CarIndicators { get; set; } = new List<CarIndicator>();
        #endregion

    }
}
