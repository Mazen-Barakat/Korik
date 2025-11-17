using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Domain
{
    public enum IndicatorType
    {
        ACService, ///Time
        CarLicenseAndEnsuranceExpiry, ///Time
        GeneralMaintenance,//Time , Mileage
        OilChange, //Mileage 
        BatteryHealth,//Time
        TireChange, //Mileage , Time
    }

    public enum CarStatus
    {
        Normal,
        Warning,
        Critical,
        //NotChecked,
        //Checked,
        UnKnown,
        //Replaced,
    }


    public class CarIndicator : BaseEntity
    {
        public IndicatorType IndicatorType { get; set; }
        public CarStatus CarStatus { get; set; }
        public DateTime LastCheckedDate { get; set; }
        public DateTime NextCheckedDate { get; set; }
        public int NextMileage { get; set; }

        public int MileageDifference { get; set; }
        public TimeSpan TimeDifference { get; set; }

        public double TimeDifferenceAsPercentage { get; set; }


        #region CarInicator M----1 Car
        [ForeignKey("Car")]
        public int CarId { get; set; }
        public Car Car { get; set; }
        #endregion
    }
}
