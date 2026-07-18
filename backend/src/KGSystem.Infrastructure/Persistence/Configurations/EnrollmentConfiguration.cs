using KGSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KGSystem.Infrastructure.Persistence.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.HasIndex(e => new { e.ChildId, e.KGPhaseId, e.AcademicYearId }).IsUnique();

        builder.Property(e => e.EnrollmentDate).IsRequired();
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(e => e.Notes).HasMaxLength(500);

        builder.HasOne(e => e.Child)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.ChildId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.KGPhase)
            .WithMany(p => p.Enrollments)
            .HasForeignKey(e => e.KGPhaseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.AcademicYear)
            .WithMany(y => y.Enrollments)
            .HasForeignKey(e => e.AcademicYearId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(e => e.DomainEvents);
    }
}
