using KGSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KGSystem.Infrastructure.Persistence.Configurations;

public class KGPhaseConfiguration : IEntityTypeConfiguration<KGPhase>
{
    public void Configure(EntityTypeBuilder<KGPhase> builder)
    {
        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.HasIndex(p => p.Code).IsUnique();

        builder.Property(p => p.Code).HasMaxLength(20).IsRequired();
        builder.Property(p => p.NameAr).HasMaxLength(100).IsRequired();
        builder.Property(p => p.NameEn).HasMaxLength(100).IsRequired();
        builder.Property(p => p.DescriptionAr).HasMaxLength(500);
        builder.Property(p => p.DescriptionEn).HasMaxLength(500);
        builder.Property(p => p.SortOrder).IsRequired();

        builder.Ignore(p => p.DomainEvents);
    }
}
