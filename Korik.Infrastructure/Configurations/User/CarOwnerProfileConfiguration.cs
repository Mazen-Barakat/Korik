using Korik.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Infrastructure
{
    public class CarOwnerProfileConfiguration : IEntityTypeConfiguration<CarOwnerProfile>
    {
        public void Configure(EntityTypeBuilder<CarOwnerProfile> builder)
        {
            builder.ToTable("CarOwnerProfiles");

            // Primary key
            builder.HasKey(p => p.Id);

            // Property constraints
            builder.Property(p => p.FirstName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(p => p.LastName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(p => p.PhoneNumber)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(p => p.Country)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.Governorate)
                   .HasMaxLength(100);

            builder.Property(p => p.City)
                   .HasMaxLength(100);

            builder.Property(p => p.ProfileImageUrl)
                   .HasMaxLength(255);

            builder.Property(p => p.PreferredLanguage)
                   .IsRequired()
                   .HasMaxLength(10);

            // Relationships
            builder.HasOne(p => p.ApplicationUser)
                   .WithOne(u => u.CarOwnerProfile)
                   .HasForeignKey<CarOwnerProfile>(p => p.ApplicationUserId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }

}
