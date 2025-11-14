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
        FuelLevel,
        TirePressure,
        OilLevel,
        BatteryHealth,
        EngineTemperature
    }

    public enum CarStatus
    {
        Normal,
        Warning,
        Critical,
        NotChecked,
        Checked,
        UnKnown,
        Replaced,
    }


    /// <summary>
    /// hoihgfdutfriugt
    /// </summary>

    public class CarIndicator : BaseEntity
    {
        public IndicatorType IndicatorType { get; set; }
        public CarStatus CarStatus { get; set; }
        public DateTime LastCheckedDate { get; set; }
        public DateTime NextCheckedDate{ get; set;}

        #region CarInicator M----1 Car
        [ForeignKey("Car")]
        public int CarId { get; set; }
        public Car Car { get; set; }
        #endregion
    }
}
