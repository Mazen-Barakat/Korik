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
    public class CarConfiguration : IEntityTypeConfiguration<Car>
    {
        public void Configure(EntityTypeBuilder<Car> builder)
        {
            // Table name
            builder.ToTable("Cars");

            // Primary Key
            builder.HasKey(c => c.Id);

            // Properties
            builder.Property(c => c.Make)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(c => c.Model)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(c => c.Year)
                   .IsRequired();

            builder.Property(c => c.EngineCapacity)
                   .IsRequired();

            builder.Property(c => c.CurrentMileage)
                   .IsRequired();

            builder.Property(c => c.LicensePlate)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(c => c.TransmissionType)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.Property(c => c.FuelType)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.Property(c => c.Origin)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            // Relationships

            // Many-to-One with CarOwnerProfile
            builder.HasOne(c => c.CarOwnerProfile)
                   .WithMany(co => co.Cars)
                   .HasForeignKey(c => c.CarOwnerProfileId)
                   .OnDelete(DeleteBehavior.NoAction);

            // One-to-Many with Bookings
            builder.HasMany(c => c.Bookings)
                   .WithOne(b => b.Car)
                   .HasForeignKey(b => b.CarId)
                   .OnDelete(DeleteBehavior.NoAction);

            // One-to-Many with CarExpenses
            builder.HasMany(c => c.CarExpenses)
                   .WithOne(ce => ce.Car)
                   .HasForeignKey(ce => ce.CarId)
                   .OnDelete(DeleteBehavior.NoAction);

            // One-to-Many with CarIndicators
            builder.HasMany(c => c.CarIndicators)
                   .WithOne(ci => ci.Car)
                   .HasForeignKey(ci => ci.CarId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Optional: Unique LicensePlate per car
            builder.HasIndex(c => c.LicensePlate).IsUnique();
        }
    }
}
