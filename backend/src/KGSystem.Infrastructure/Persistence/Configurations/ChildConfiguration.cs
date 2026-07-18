using KGSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KGSystem.Infrastructure.Persistence.Configurations;

public class ChildConfiguration : IEntityTypeConfiguration<Child>
{
    public void Configure(EntityTypeBuilder<Child> builder)
    {
        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.Property(c => c.FirstNameAr).HasMaxLength(100).IsRequired();
        builder.Property(c => c.FirstNameEn).HasMaxLength(100).IsRequired();
        builder.Property(c => c.LastNameAr).HasMaxLength(100).IsRequired();
        builder.Property(c => c.LastNameEn).HasMaxLength(100).IsRequired();
        builder.Property(c => c.DateOfBirth).IsRequired();
        builder.Property(c => c.Gender).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(c => c.Nationality).HasMaxLength(100);
        builder.Property(c => c.GuardianNameAr).HasMaxLength(200).IsRequired();
        builder.Property(c => c.GuardianNameEn).HasMaxLength(200).IsRequired();
        builder.Property(c => c.GuardianPhone).HasMaxLength(50).IsRequired();
        builder.Property(c => c.GuardianEmail).HasMaxLength(200);
        builder.Property(c => c.Address).HasMaxLength(500);
        builder.Property(c => c.PhotoUrl).HasMaxLength(500);
        builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(c => c.EnrollmentDate).IsRequired();

        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.GuardianPhone);

        builder.Ignore(c => c.DomainEvents);
    }
}
