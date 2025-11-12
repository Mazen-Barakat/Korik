using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Domain
{
    public class WorshopService : BaseEntity
    {
        public decimal Price { get; set; }  
        public int Duration { get; set; }


        #region WorshopService M---1 Service
        [ForeignKey("Service")]
        public int ServiceId { get; set; }
        public Service Service { get; set; }
        #endregion

        #region WorshopService M---1 Workshop
        [ForeignKey("WorkShopProfile")]
        public int WorkShopProfileId { get; set; }
        public WorkShopProfile WorkShopProfile { get; set; }
        #endregion
    }
}
