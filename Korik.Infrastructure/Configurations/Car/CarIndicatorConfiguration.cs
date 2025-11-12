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
    public class CarIndicatorConfiguration : IEntityTypeConfiguration<CarIndicator>
    {
        public void Configure(EntityTypeBuilder<CarIndicator> builder)
        {
            // Table name
            builder.ToTable("CarIndicators");

            // Primary Key
            builder.HasKey(ci => ci.Id);

            // Properties
            builder.Property(ci => ci.IndicatorType)
                   .IsRequired()
                   .HasConversion<string>() // store enum as string for readability
                   .HasMaxLength(50);

            builder.Property(ci => ci.CarStatus)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(50);

            builder.Property(ci => ci.LastCheckedDate)
                   .IsRequired();

            builder.Property(ci => ci.NextCheckedDate)
                   .IsRequired();

            // Relationships
            builder.HasOne(ci => ci.Car)
                   .WithMany(c => c.CarIndicators)
                   .HasForeignKey(ci => ci.CarId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Optional: enforce unique indicator type per car
            builder.HasIndex(ci => new { ci.CarId, ci.IndicatorType })
                   .IsUnique();
        }
    }
}
