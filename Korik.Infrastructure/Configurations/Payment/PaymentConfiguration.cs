using Korik.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Korik.Infrastructure
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.TotalAmount)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.Property(p => p.CommissionAmount)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.Property(p => p.WorkshopAmount)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.Property(p => p.CommissionRate)
                .IsRequired()
                .HasPrecision(5, 4);

            builder.Property(p => p.StripePaymentStatus)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(p => p.StripePaymentIntentId)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(p => p.IsPaidOut)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(p => p.PayoutMethod)
                .HasMaxLength(50);

            builder.Property(p => p.PayoutReference)
                .HasMaxLength(255);

            builder.Property(p => p.PayoutNotes)
                .HasMaxLength(1000);

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            // Relationships
            builder.HasOne(p => p.Booking)
                .WithMany()
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(p => p.StripePaymentIntentId)
                .IsUnique();
            
            builder.HasIndex(p => p.IsPaidOut);
        }
    }
}
