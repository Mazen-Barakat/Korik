using Korik.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class CreateSubcategoryDTO
    {
        public string Name { get; set; }
        public int CategoryId { get; set; }
    }
}
