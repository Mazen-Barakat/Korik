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
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            // Table name
            builder.ToTable("Reviews");

            // Primary Key
            builder.HasKey(r => r.Id);

            // Properties
            builder.Property(r => r.Rating)
                   .IsRequired();

            builder.Property(r => r.Comment)
                   .HasMaxLength(1000);

            builder.Property(r => r.PaidAmount)
                   .HasPrecision(10, 2);

            builder.Property(r => r.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            // Relationships

            // One-to-One with Booking
            builder.HasOne(r => r.Booking)
                   .WithOne(b => b.Review)
                   .HasForeignKey<Review>(r => r.BookingId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Optional: enforce unique BookingId (redundant due to 1:1, but explicit)
            builder.HasIndex(r => r.BookingId).IsUnique();

            // Many-to-One with WorkShopProfile
            builder.HasOne(r => r.WorkShopProfile)
                   .WithMany(wp => wp.Reviews)
                   .HasForeignKey(r => r.WorkShopProfileId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Many-to-One with CarOwnerProfile
            builder.HasOne(r => r.CarOwnerProfile)
                   .WithMany(cop => cop.Reviews)
                   .HasForeignKey(r => r.CarOwnerProfileId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
