using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Domain
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string IconURL { get; set; }
        public int DisplayOrder { get; set; }

        #region Category 1----M Subcategory 
        public ICollection<Subcategory> Subcategories { get; set; } = new List<Subcategory>();
        #endregion

    }
}
