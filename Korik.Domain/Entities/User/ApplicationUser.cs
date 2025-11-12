using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public string? FullAddress { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }


        #region Navigation Properties
        public virtual CarOwnerProfile? CarOwnerProfile { get; set; }
        public virtual WorkShopProfile? WorkShopProfile { get; set; }
        #endregion
    }
}
