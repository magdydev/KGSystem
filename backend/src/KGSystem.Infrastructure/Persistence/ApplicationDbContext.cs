using KGSystem.Domain.Entities;
using KGSystem.Infrastructure.Persistence.Interceptors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KGSystem.Infrastructure.Persistence;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    AuditableEntitySaveChangesInterceptor auditInterceptor)
    : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<Child> Children => Set<Child>();
    public DbSet<KGPhase> KGPhases => Set<KGPhase>();
    public DbSet<AcademicYear> AcademicYears => Set<AcademicYear>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<MonthlyFee> MonthlyFees => Set<MonthlyFee>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<BrandingSetting> BrandingSettings => Set<BrandingSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(auditInterceptor);
        base.OnConfiguring(optionsBuilder);
    }
}
