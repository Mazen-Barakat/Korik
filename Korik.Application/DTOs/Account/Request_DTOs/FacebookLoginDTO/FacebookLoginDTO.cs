using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class FacebookLoginDTO
    {
        public string AccessToken { get; set; } = string.Empty;
        public string Role { get; set; }
    }

}
