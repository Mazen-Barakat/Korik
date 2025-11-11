using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Domain
{
    public class WorkingHours : BaseEntity
    {
        public DayOfWeek Day { get; set; }
        public TimeOnly From { get; set; }
        public TimeOnly To { get; set; }
        public bool IsClosed { get; set; }


        // Foreign Key
        [ForeignKey("WorkShopProfile")]
        public int WorkShopProfileId { get; set; }
        public virtual WorkShopProfile WorkShopProfile { get; set; } = null!;
    }
}
