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
    public class WorkShopPhotoConfiguration : IEntityTypeConfiguration<WorkShopPhoto>
    {
        public void Configure(EntityTypeBuilder<WorkShopPhoto> builder)
        {
            // Table name
            builder.ToTable("WorkShopPhotos");

            // Primary Key
            builder.HasKey(wp => wp.Id);

            // Properties
            builder.Property(wp => wp.PhotoUrl)
                   .IsRequired()
                   .HasMaxLength(500);

            // Relationships
            builder.HasOne(wp => wp.WorkShopProfile)
                   .WithMany(ws => ws.WorkShopPhotos)
                   .HasForeignKey(wp => wp.WorkShopProfileId)
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
