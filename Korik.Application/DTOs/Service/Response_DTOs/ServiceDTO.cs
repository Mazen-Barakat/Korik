using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class ServiceDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int SubcategoryId { get; set; }

        //optional: Include SubcategoryName if needed
        public string? SubcategoryName { get; set; }
    }
}
