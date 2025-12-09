using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class AiChatDTO
    {
        public string Response { get; set; } = string.Empty;

        public bool IsSuccess { get; set; }

        // Helpful for debugging: tells the frontend if the AI took a specific action
        // e.g., "BookingCreated", "MaintenanceChecked"
        public string? ActionTaken { get; set; }
    }
}
