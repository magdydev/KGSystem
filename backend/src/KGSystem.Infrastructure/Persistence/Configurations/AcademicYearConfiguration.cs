using KGSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KGSystem.Infrastructure.Persistence.Configurations;

public class AcademicYearConfiguration : IEntityTypeConfiguration<AcademicYear>
{
    public void Configure(EntityTypeBuilder<AcademicYear> builder)
    {
        builder.HasQueryFilter(y => !y.IsDeleted);

        builder.HasIndex(y => y.Code).IsUnique();
        builder.HasIndex(y => y.IsActive);

        builder.Property(y => y.Code).HasMaxLength(20).IsRequired();
        builder.Property(y => y.NameAr).HasMaxLength(100).IsRequired();
        builder.Property(y => y.NameEn).HasMaxLength(100).IsRequired();
        builder.Property(y => y.StartDate).IsRequired();
        builder.Property(y => y.EndDate).IsRequired();

        builder.Ignore(y => y.DomainEvents);
    }
}
