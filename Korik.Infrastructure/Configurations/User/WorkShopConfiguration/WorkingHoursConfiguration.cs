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
    public class WorkingHoursConfiguration : IEntityTypeConfiguration<WorkingHours>
    {
        public void Configure(EntityTypeBuilder<WorkingHours> builder)
        {
            // Table name
            builder.ToTable("WorkingHours");

            // Primary Key
            builder.HasKey(wh => wh.Id);

            // Properties
            builder.Property(wh => wh.Day)
                   .IsRequired()
                   .HasConversion<string>() // Store DayOfWeek as string
                   .HasMaxLength(20);

            builder.Property(wh => wh.From)
                   .IsRequired()
                   .HasConversion(
                       v => v.ToTimeSpan(),     // Store TimeOnly as TimeSpan in DB
                       v => TimeOnly.FromTimeSpan(v)
                   );

            builder.Property(wh => wh.To)
                   .IsRequired()
                   .HasConversion(
                       v => v.ToTimeSpan(),
                       v => TimeOnly.FromTimeSpan(v)
                   );

            builder.Property(wh => wh.IsClosed)
                   .IsRequired();

            // Relationships
            builder.HasOne(wh => wh.WorkShopProfile)
                   .WithMany(ws => ws.WorkingHours)
                   .HasForeignKey(wh => wh.WorkShopProfileId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Optional: unique constraint to prevent duplicate day entries per workshop
            builder.HasIndex(wh => new { wh.WorkShopProfileId, wh.Day }).IsUnique();
        }
    }
}
