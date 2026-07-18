using KGSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KGSystem.Infrastructure.Persistence.Configurations;

public class BrandingSettingConfiguration : IEntityTypeConfiguration<BrandingSetting>
{
    public void Configure(EntityTypeBuilder<BrandingSetting> builder)
    {
        builder.HasQueryFilter(b => !b.IsDeleted);

        builder.Property(b => b.AppName).HasMaxLength(200).IsRequired();
        builder.Property(b => b.AppNameAr).HasMaxLength(200).IsRequired();
        builder.Property(b => b.LogoUrl).HasMaxLength(2048);
        builder.Property(b => b.LogoData).HasColumnType("nvarchar(max)");
        builder.Property(b => b.PrimaryColor).HasMaxLength(20).IsRequired();
        builder.Property(b => b.SecondaryColor).HasMaxLength(20).IsRequired();
        builder.Property(b => b.Currency).HasMaxLength(3).IsRequired();

        builder.Ignore(b => b.DomainEvents);
    }
}
