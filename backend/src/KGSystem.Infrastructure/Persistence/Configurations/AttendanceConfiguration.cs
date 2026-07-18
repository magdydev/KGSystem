using KGSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KGSystem.Infrastructure.Persistence.Configurations;

public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder.HasQueryFilter(a => !a.IsDeleted);

        builder.HasIndex(a => new { a.ChildId, a.Date }).IsUnique();

        builder.Property(a => a.Date).IsRequired();
        builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(a => a.Notes).HasMaxLength(500);

        builder.HasOne(a => a.Child)
            .WithMany(c => c.Attendances)
            .HasForeignKey(a => a.ChildId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(a => a.DomainEvents);
    }
}
