using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class CarIndicatorDTO
    {
        public int Id { get; set; }
        public IndicatorType IndicatorType { get; set; }
        public CarStatus CarStatus { get; set; }
        public DateTime LastCheckedDate { get; set; }
        public DateTime NextCheckedDate { get; set; }
        public int CarId { get; set; }
    }
}
