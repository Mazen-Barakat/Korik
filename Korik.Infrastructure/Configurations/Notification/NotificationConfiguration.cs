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
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.SenderId)
                  .IsRequired()
            .HasMaxLength(450);

            builder.Property(n => n.ReceiverId)
                 .IsRequired()
                     .HasMaxLength(450);

            builder.Property(n => n.Message)
                 .IsRequired()
                .HasMaxLength(500);

            builder.Property(n => n.Type)
              .IsRequired()
                     .HasConversion<int>();

            builder.Property(n => n.IsRead)
                 .IsRequired()
                        .HasDefaultValue(false);

            builder.Property(n => n.CreatedAt)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");

                      // Relationships
            builder.HasOne(n => n.Sender)
               .WithMany()
                  .HasForeignKey(n => n.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(n => n.Receiver)
                 .WithMany()
             .HasForeignKey(n => n.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(n => n.Booking)
                   .WithMany()
                  .HasForeignKey(n => n.BookingId)
                  .OnDelete(DeleteBehavior.SetNull)
                   .IsRequired(false);

                   // Indexes for performance
            builder.HasIndex(n => n.ReceiverId);
            builder.HasIndex(n => n.IsRead);
            builder.HasIndex(n => n.CreatedAt);
        }
    }
}
