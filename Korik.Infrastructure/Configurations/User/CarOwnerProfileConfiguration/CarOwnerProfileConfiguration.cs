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
            // Table name
            builder.ToTable("CarOwnerProfiles");

            // Primary Key
            builder.HasKey(co => co.Id);

            // Properties
            builder.Property(co => co.FirstName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(co => co.LastName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(co => co.PhoneNumber)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(co => co.Country)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(co => co.Governorate)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(co => co.City)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(co => co.ProfileImageUrl)
                   .HasMaxLength(300);

            builder.Property(co => co.PreferredLanguage)
                   .IsRequired()
                   .HasConversion<string>() // Store enum as string for readability
                   .HasMaxLength(20);

            // Relationships
            builder.HasOne(co => co.ApplicationUser)
                   .WithOne(co => co.CarOwnerProfile)
                   .HasForeignKey<CarOwnerProfile>(co => co.ApplicationUserId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(co => co.Cars)
                   .WithOne(c => c.CarOwnerProfile)
                   .HasForeignKey(c => c.CarOwnerProfileId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Optional: add an index for faster lookups by ApplicationUserId
            builder.HasIndex(co => co.ApplicationUserId).IsUnique();
        }
    }
}
