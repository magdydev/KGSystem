using KGSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KGSystem.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.HasIndex(p => new { p.EnrollmentId, p.Month, p.Year });
        builder.HasIndex(p => p.Status);

        builder.Property(p => p.Month).IsRequired();
        builder.Property(p => p.Year).IsRequired();
        builder.Property(p => p.AmountDue).HasPrecision(18, 2).IsRequired();
        builder.Property(p => p.AmountPaid).HasPrecision(18, 2).IsRequired();
        builder.Property(p => p.Discount).HasPrecision(18, 2).IsRequired();
        builder.Property(p => p.DueDate).IsRequired();
        builder.Property(p => p.PaidDate);
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(p => p.Method).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(p => p.Notes).HasMaxLength(500);
        builder.Property(p => p.ReceivedBy).HasMaxLength(100);

        builder.HasOne(p => p.Enrollment)
            .WithMany()
            .HasForeignKey(p => p.EnrollmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(p => p.DomainEvents);
    }
}
