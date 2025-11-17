using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class UpdateWorkShopProfileStatusDTO
    {
        public int Id { get; set; }
        public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
    }
}