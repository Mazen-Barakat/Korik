using Korik.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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

            // Custom IndicatorType converter with fallback for legacy values
            var indicatorTypeConverter = new ValueConverter<IndicatorType, string>(
                // Convert enum to string for database
                v => v.ToString(),
                // Convert string from database to enum with fallback
                v => ConvertToIndicatorType(v)
            );

            builder.Property(ci => ci.IndicatorType)
                   .HasConversion(indicatorTypeConverter)
                   .IsRequired();

            // Custom CarStatus converter with fallback for legacy values
            var carStatusConverter = new ValueConverter<CarStatus, string>(
                // Convert enum to string for database
                v => v.ToString(),
                // Convert string from database to enum with fallback
                v => ConvertToCarStatus(v)
            );

            builder.Property(ci => ci.CarStatus)
                   .HasConversion(carStatusConverter)
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

        /// <summary>
        /// Converts database string value to IndicatorType enum with fallback handling for legacy values
        /// </summary>
        private static IndicatorType ConvertToIndicatorType(string value)
        {
            // Handle legacy 'Oil' value by mapping it to 'OilChange'
            if (string.Equals(value, "Oil", StringComparison.OrdinalIgnoreCase))
            {
                return IndicatorType.OilChange;
            }

            // Try to parse the value to the enum
            if (Enum.TryParse<IndicatorType>(value, ignoreCase: true, out var type))
            {
                return type;
            }

            // Fallback for any unknown values - default to GeneralMaintenance
            return IndicatorType.GeneralMaintenance;
        }

        /// <summary>
        /// Converts database string value to CarStatus enum with fallback handling for legacy values
        /// </summary>
        private static CarStatus ConvertToCarStatus(string value)
        {
            // Handle legacy 'Good' value by mapping it to 'Normal'
            if (string.Equals(value, "Good", StringComparison.OrdinalIgnoreCase))
            {
                return CarStatus.Normal;
            }

            // Try to parse the value to the enum
            if (Enum.TryParse<CarStatus>(value, ignoreCase: true, out var status))
            {
                return status;
            }

            // Fallback for any unknown values
            return CarStatus.UnKnown;
        }
    }
}
