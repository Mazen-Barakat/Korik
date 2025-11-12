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
    
    public class BookingPhotoConfiguration : IEntityTypeConfiguration<BookingPhoto>
    {
        public void Configure(EntityTypeBuilder<BookingPhoto> builder)
        {
            // Table name
            builder.ToTable("BookingPhotos");

            // Primary Key
            builder.HasKey(bp => bp.Id);

            // Properties
            builder.Property(bp => bp.PhotoUrl)
                   .IsRequired()
                   .HasMaxLength(500);

            // Relationships
            builder.HasOne(bp => bp.Booking)
                   .WithMany(b => b.BookingPhotos)
                   .HasForeignKey(bp => bp.BookingId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
