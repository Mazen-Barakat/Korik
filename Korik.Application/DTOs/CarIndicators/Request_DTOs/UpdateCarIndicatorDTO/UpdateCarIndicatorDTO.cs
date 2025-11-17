using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class UpdateCarIndicatorDTO
    {
        public int Id { get; set; } // Existing indicator ID

        public IndicatorType IndicatorType { get; set; }
        public int NextMileage { get; set; }
        public DateTime LastCheckedDate { get; set; }
        public DateTime NextCheckedDate { get; set; }
        public int CarId { get; set; }

        [JsonIgnore]
        public CarStatus CarStatus { get; set; }

        [JsonIgnore]
        public int MileageDifference { get; set; }

        [JsonIgnore]
        public TimeSpan TimeDifference { get; set; }

        [JsonIgnore]
        public double TimeDifferenceAsPercentage { get; set; }
    }
}
