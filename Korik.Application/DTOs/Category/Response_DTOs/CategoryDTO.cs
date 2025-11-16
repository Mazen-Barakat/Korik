using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IconURL { get; set; }
        public int DisplayOrder { get; set; }
    }
}
