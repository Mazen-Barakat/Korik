using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class CreateWorkShopPhotosDTO
    {
        public int WorkShopProfileId { get; set; }
        public List<IFormFile> Photos { get; set; } = new();
    }
}