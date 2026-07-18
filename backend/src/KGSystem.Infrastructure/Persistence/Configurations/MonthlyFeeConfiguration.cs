using KGSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KGSystem.Infrastructure.Persistence.Configurations;

public class MonthlyFeeConfiguration : IEntityTypeConfiguration<MonthlyFee>
{
    public void Configure(EntityTypeBuilder<MonthlyFee> builder)
    {
        builder.HasQueryFilter(f => !f.IsDeleted);

        builder.HasIndex(f => new { f.AcademicYearId, f.Month }).IsUnique();

        builder.Property(f => f.Month).IsRequired();
        builder.Property(f => f.DueDate);

        builder.OwnsOne(f => f.Amount, money =>
        {
            money.Property(m => m.Amount).HasColumnName("FeeAmount").HasPrecision(18, 2).IsRequired();
            money.Property(m => m.Currency).HasColumnName("FeeCurrency").HasMaxLength(3).IsRequired();
        });

        builder.HasOne(f => f.AcademicYear)
            .WithMany(y => y.MonthlyFees)
            .HasForeignKey(f => f.AcademicYearId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(f => f.DomainEvents);
    }
}
