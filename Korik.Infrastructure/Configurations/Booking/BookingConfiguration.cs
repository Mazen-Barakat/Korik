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
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            // Table name
            builder.ToTable("Bookings");

            // Primary Key
            builder.HasKey(b => b.Id);

            // Properties
            builder.Property(b => b.Status)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(50);

            builder.Property(b => b.PaymentMethod)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(50);

            builder.Property(b => b.PaymentStatus)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(50);

            builder.Property(b => b.IssueDescription)
                   .HasMaxLength(1000);

            builder.Property(b => b.PaidAmount)
                   .HasPrecision(10, 2);

            builder.Property(b => b.AppointmentDate)
                   .IsRequired();

            builder.Property(b => b.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            // Relationships

            // Many-to-One with Car
            builder.HasOne(b => b.Car)
                   .WithMany(c => c.Bookings)
                   .HasForeignKey(b => b.CarId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Many-to-One with WorkShopProfile
            builder.HasOne(b => b.WorkShopProfile)
                   .WithMany(w => w.Bookings)
                   .HasForeignKey(b => b.WorkShopProfileId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Many-to-One with Service
            builder.HasOne(b => b.Service)
                   .WithMany(s => s.Bookings)
                   .HasForeignKey(b => b.ServiceId)
                   .OnDelete(DeleteBehavior.NoAction);

            // One-to-One with Review
            builder.HasOne(b => b.Review)
                   .WithOne(r => r.Booking)
                   .HasForeignKey<Review>(r => r.BookingId)
                   .OnDelete(DeleteBehavior.NoAction);

            // One-to-Many with BookingPhotos
            builder.HasMany(b => b.BookingPhotos)
                   .WithOne(bp => bp.Booking)
                   .HasForeignKey(bp => bp.BookingId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
