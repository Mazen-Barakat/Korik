using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class WorkShopWorkingHoursDTO
    {
        public int Id { get; set; }
        public DayOfWeek Day { get; set; }
        public TimeOnly From { get; set; }
        public TimeOnly To { get; set; }
        public bool IsClosed { get; set; }
        public int WorkShopProfileId { get; set; }
    }
}
