using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class WorkShopPhotoItemDTO
    {
        public int Id { get; set; }
        public string PhotoUrl { get; set; }
    }

    public class WorkShopPhotosDTO
    {
        public int WorkShopProfileId { get; set; }
        public List<WorkShopPhotoItemDTO> Photos { get; set; } = new();
    }
}