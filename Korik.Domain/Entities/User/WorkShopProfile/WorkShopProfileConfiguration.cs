using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Korik.Domain
{
    public class WorkShopProfileConfiguration : IEntityTypeConfiguration<WorkShopProfile>
    {
        public void Configure(EntityTypeBuilder<WorkShopProfile> builder)
        {
            builder.ToTable("WorkShopProfiles");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.Description)
                .IsRequired();

            builder.Property(x => x.LicenceImageUrl)
                .IsRequired();

            builder.Property(x => x.PhoneNumber)
                .IsRequired();

            builder.Property(x => x.Country)
                .IsRequired();

            builder.Property(x => x.Governorate)
                .IsRequired();

            builder.Property(x => x.City)
                .IsRequired();

            builder.Property(x => x.LogoImageUrl)
                .IsRequired(false);

            builder.Property(x => x.Rating)
                .HasColumnType("float");

            builder.Property(x => x.Latitude)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Longitude)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetime2");

            builder.Property(x => x.UpdatedAt)
                .HasColumnType("datetime2");

            builder.Property(x => x.WorkShopType)
                .HasColumnType("int");

            builder.Property(x => x.VerificationStatus)
                .HasColumnType("int");

            // ApplicationUser relation (1-1 or 1-many depending on ApplicationUser mapping)
            builder.Property(x => x.ApplicationUserId)
                .IsRequired()
                .HasMaxLength(450);

            builder.HasOne(x => x.ApplicationUser)
                .WithMany()
                .HasForeignKey(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // One WorkShopProfile has many WorkShopPhotos
            builder.HasMany(x => x.WorkShopPhotos)
                .WithOne(p => p.WorkShopProfile)
                .HasForeignKey(p => p.WorkShopProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // One WorkShopProfile has many WorkingHours
            builder.HasMany(x => x.WorkingHours)
                .WithOne(wh => wh.WorkShopProfile)
                .HasForeignKey(wh => wh.WorkShopProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // One WorkShopProfile has many Bookings
            builder.HasMany(x => x.Bookings)
                .WithOne(b => b.WorkShopProfile)
                .HasForeignKey(b => b.WorkShopProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // One WorkShopProfile has many WorshopServices
            builder.HasMany(x => x.WorshopServices)
                .WithOne(ws => ws.WorkShopProfile)
                .HasForeignKey(ws => ws.WorkShopProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // index on ApplicationUserId (matches snapshot)
            builder.HasIndex(x => x.ApplicationUserId);
        }
    }
}
