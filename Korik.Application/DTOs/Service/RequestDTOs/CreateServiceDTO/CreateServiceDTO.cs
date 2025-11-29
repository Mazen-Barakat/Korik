using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class CreateServiceDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int SubcategoryId { get; set; }
    }

}
