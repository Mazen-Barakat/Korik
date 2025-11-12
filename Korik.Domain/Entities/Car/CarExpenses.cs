using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Domain
{
    public enum ExpenseType
    {
        Fuel,
        Maintenance,
        Repair,
        Insurance,
        Other
    }

    public class CarExpenses : BaseEntity
    {
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime ExpenseDate { get; set; }
        public ExpenseType ExpenseType { get; set; }

        #region CarExpenses M----1 Car
        [ForeignKey("Car")]
        public int CarId { get; set; }
        public Car Car { get; set; }
        #endregion

    }
}
