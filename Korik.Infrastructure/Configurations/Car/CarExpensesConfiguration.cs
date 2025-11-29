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
    public class CarExpensesConfiguration : IEntityTypeConfiguration<CarExpenses>
    {
        public void Configure(EntityTypeBuilder<CarExpenses> builder)
        {
            // Table name
            builder.ToTable("CarExpenses");

            // Primary Key
            builder.HasKey(ce => ce.Id);

            // Properties
            builder.Property(ce => ce.Amount)
                   .IsRequired()
                   .HasPrecision(10, 2);

            builder.Property(ce => ce.Description)
                   .HasMaxLength(500);

            builder.Property(ce => ce.ExpenseDate)
                   .IsRequired();

            builder.Property(ce => ce.ExpenseType)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(50);

            // Relationships
            builder.HasOne(ce => ce.Car)
                   .WithMany(c => c.CarExpenses)
                   .HasForeignKey(ce => ce.CarId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
