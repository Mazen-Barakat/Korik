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
    public class WorkshopServiceConfiguration : IEntityTypeConfiguration<WorshopService>
    {
        public void Configure(EntityTypeBuilder<WorshopService> builder)
        {
            // Table name
            builder.ToTable("WorkshopServices");

            // Primary Key
            builder.HasKey(ws => ws.Id);

            // Properties

            builder.Property(ws => ws.Duration)
                     .IsRequired(); // duration in minutes

            builder.Property(ws => ws.MinPrice)
                   .IsRequired()
                   .HasPrecision(10, 2);

            builder.Property(ws => ws.MaxPrice)
                   .IsRequired()
                   .HasPrecision(10, 2);

            builder.Property(ws => ws.Origin)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(50);



            // Relationships

            // Many-to-One with Service
            builder.HasOne(ws => ws.Service)
                   .WithMany(s => s.WorshopServices)
                   .HasForeignKey(ws => ws.ServiceId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Many-to-One with WorkShopProfile
            builder.HasOne(ws => ws.WorkShopProfile)
                   .WithMany(w => w.WorshopServices)
                   .HasForeignKey(ws => ws.WorkShopProfileId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Optional: enforce unique combination of Service + Workshop
            builder.HasIndex(ws => new { ws.ServiceId, ws.WorkShopProfileId })
                   .IsUnique();
        }
    }
}
