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

            // Primary Key (inherited from BaseEntity)
            builder.HasKey(ci => ci.Id);

            // Enum properties as string (optional, better readability)
            builder.Property(ci => ci.IndicatorType)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(ci => ci.CarStatus)
                   .HasConversion<string>()
                   .IsRequired();

            // DateTime properties
            builder.Property(ci => ci.LastCheckedDate)
                   .IsRequired();

            builder.Property(ci => ci.NextCheckedDate)
                   .IsRequired();

            // Mileage properties
            builder.Property(ci => ci.NextMileage)
                   .IsRequired();

            builder.Property(ci => ci.MileageDifference)
                   .IsRequired();

            // TimeSpan and double properties
            builder.Property(ci => ci.TimeDifference)
                   .HasConversion(
                       v => v.Ticks,          // store TimeSpan as long
                       v => TimeSpan.FromTicks(v))
                   .IsRequired();

            builder.Property(ci => ci.TimeDifferenceAsPercentage)
                   .IsRequired();

            // Relationships
            builder.HasOne(ci => ci.Car)
                   .WithMany(c => c.CarIndicators)
                   .HasForeignKey(ci => ci.CarId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
