using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Domain
{
    public class WorkShopPhoto : BaseEntity
    {
        public string PhotoUrl { get; set; } = string.Empty;

        // Foreign Key
        [ForeignKey("WorkShopProfile")]
        public int WorkShopProfileId { get; set; }
        public virtual WorkShopProfile WorkShopProfile { get; set; } = null!;

    }
}
