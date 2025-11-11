using Korik.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Infrastructure
{
    public class Korik : IdentityDbContext<ApplicationUser>
    {
        #region CTORs
        public Korik() : base() { }

        public Korik(DbContextOptions<Korik> options) : base(options)
        {
        }
        #endregion


        #region DbSets
        public virtual DbSet<CarOwnerProfile> CarOwnerProfiles { get; set; }
       
        #region WorkShop
        public virtual DbSet<WorkShopProfile> WorkShopProfiles { get; set; }
        public virtual DbSet<WorkShopPhoto> WorkShopPhotos { get; set; }
        public virtual DbSet<WorkingHours> WorkingHours { get; set; } 
        #endregion
       
        public virtual DbSet<Car> Cars { get; set; }

        #endregion


        #region Fluent API
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationUser).Assembly);

            base.OnModelCreating(builder);
        }
        #endregion
    }
}
