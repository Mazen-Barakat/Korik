using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class CreateAiChatDTO
    {
        public string Message { get; set; } = string.Empty;

        public string? SessionId { get; set; }
    }
}
