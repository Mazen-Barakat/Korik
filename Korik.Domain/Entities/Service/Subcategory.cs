using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Domain
{
    public class Subcategory : BaseEntity
    {
        public string Name { get; set; }
 
        #region Subcategory 1----M Service 
        public ICollection<Service> Services { get; set; } = new List<Service>();
        #endregion

        #region Subcategory M----1 Category

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        #endregion
    }
}
