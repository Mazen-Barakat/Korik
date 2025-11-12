using Korik.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Infrastructurec
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            // Table name
            builder.ToTable("Services");

            // Primary Key
            builder.HasKey(s => s.Id);

            // Properties
            builder.Property(s => s.Name)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(s => s.Description)
                   .HasMaxLength(1000);

            builder.Property(s => s.Duration)
                   .IsRequired(); // duration in minutes

            builder.Property(s => s.MinPrice)
                   .IsRequired()
                   .HasPrecision(10, 2);

            builder.Property(s => s.MaxPrice)
                   .IsRequired()
                   .HasPrecision(10, 2);

            builder.Property(s => s.ImageUrl)
                   .HasMaxLength(300);

            // Relationships

            // Many-to-One with Subcategory
            builder.HasOne(s => s.Subcategory)
                   .WithMany(sc => sc.Services)
                   .HasForeignKey(s => s.SubcategoryId)
                   .OnDelete(DeleteBehavior.NoAction);

            // One-to-Many with Booking
            builder.HasMany(s => s.Bookings)
                   .WithOne(b => b.Service)
                   .HasForeignKey(b => b.ServiceId)
                   .OnDelete(DeleteBehavior.NoAction);

            // One-to-Many with WorkshopService
            builder.HasMany(s => s.WorshopServices)
                   .WithOne(ws => ws.Service)
                   .HasForeignKey(ws => ws.ServiceId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
