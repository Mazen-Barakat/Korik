using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class ResetPasswordDTO
    {
        public string Email { get; set; }
        public string PasswordResetToken { get; set; }
        public string NewPassword { get; set; }
    }
}
