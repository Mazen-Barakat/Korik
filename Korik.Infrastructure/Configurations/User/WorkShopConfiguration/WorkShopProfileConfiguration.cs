using Korik.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Korik.Infrastructure
{
    public class WorkShopProfileConfiguration : IEntityTypeConfiguration<WorkShopProfile>
    {
        public void Configure(EntityTypeBuilder<WorkShopProfile> builder)
        {
            // Table Name
            builder.ToTable("WorkShopProfiles");

            // Primary Key
            builder.HasKey(w => w.Id);

            // Properties
            builder.Property(w => w.Name)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(w => w.Description)
                   .HasMaxLength(1000);

            builder.Property(w => w.WorkShopType)
                   .IsRequired()
                   .HasConversion<string>() // store enum as readable string
                   .HasMaxLength(50);

            builder.Property(w => w.Country)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(w => w.Governorate)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(w => w.City)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(w => w.Rating)
                   .HasDefaultValue(0)
                   .HasPrecision(3, 2); // example: 4.75 rating

            builder.Property(w => w.VerificationStatus)
                   .IsRequired()
                   .HasConversion<string>() // store enum as string
                   .HasMaxLength(20)
                   .HasDefaultValue(VerificationStatus.Pending);

            builder.Property(w => w.LicenceImageUrl)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.Property(w => w.LogoImageUrl)
                   .HasMaxLength(300);

            builder.Property(w => w.PhoneNumber)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(w => w.Latitude)
                   .HasPrecision(10, 6);

            builder.Property(w => w.Longitude)
                   .HasPrecision(10, 6);

            builder.Property(w => w.NumbersOfTechnicians)
                   .IsRequired();

            builder.Property(w => w.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(w => w.UpdatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            // Relationships

            // 1 : 1  ->  ApplicationUser
            builder.HasOne(w => w.ApplicationUser)
                   .WithOne(w => w.WorkShopProfile)
                   .HasForeignKey<WorkShopProfile>(w => w.ApplicationUserId)
                   .OnDelete(DeleteBehavior.NoAction);

            // 1 : M  ->  WorkShopPhoto
            builder.HasMany(w => w.WorkShopPhotos)
                   .WithOne(p => p.WorkShopProfile)
                   .HasForeignKey(p => p.WorkShopProfileId)
                   .OnDelete(DeleteBehavior.Cascade);

            // 1 : M  ->  WorkingHours
            builder.HasMany(w => w.WorkingHours)
                   .WithOne(h => h.WorkShopProfile)
                   .HasForeignKey(h => h.WorkShopProfileId)
                   .OnDelete(DeleteBehavior.Cascade);

            // 1 : M  ->  Booking
            builder.HasMany(w => w.Bookings)
                   .WithOne(b => b.WorkShopProfile)
                   .HasForeignKey(b => b.WorkShopProfileId)
                   .OnDelete(DeleteBehavior.NoAction);

            // M : M  ->  WorkshopService
            builder.HasMany(w => w.WorkshopServices)
                   .WithOne(ws => ws.WorkShopProfile)
                   .HasForeignKey(ws => ws.WorkShopProfileId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Index for performance
            builder.HasIndex(w => w.ApplicationUserId).IsUnique();
            builder.HasIndex(w => new { w.City, w.WorkShopType });
        }
    }
}