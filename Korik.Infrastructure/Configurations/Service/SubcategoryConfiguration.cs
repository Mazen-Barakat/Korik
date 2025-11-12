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
    public class SubcategoryConfiguration : IEntityTypeConfiguration<Subcategory>
    {
        public void Configure(EntityTypeBuilder<Subcategory> builder)
        {
            // Table name
            builder.ToTable("Subcategories");

            // Primary Key
            builder.HasKey(sc => sc.Id);

            // Properties
            builder.Property(sc => sc.Name)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(sc => sc.Description)
                   .HasMaxLength(1000);

            // Relationships

            // Many-to-One with Category
            builder.HasOne(sc => sc.Category)
                   .WithMany(c => c.Subcategories)
                   .HasForeignKey(sc => sc.CategoryId)
                   .OnDelete(DeleteBehavior.NoAction);

            // One-to-Many with Services
            builder.HasMany(sc => sc.Services)
                   .WithOne(s => s.Subcategory)
                   .HasForeignKey(s => s.SubcategoryId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Optional: enforce unique subcategory names per category
            builder.HasIndex(sc => new { sc.CategoryId, sc.Name })
                   .IsUnique();
        }
    }
}
