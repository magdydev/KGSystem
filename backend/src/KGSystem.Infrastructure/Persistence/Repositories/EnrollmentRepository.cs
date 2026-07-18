using KGSystem.Domain.Entities;
using KGSystem.Domain.Enums;
using KGSystem.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KGSystem.Infrastructure.Persistence.Repositories;

public sealed class EnrollmentRepository(ApplicationDbContext context) : RepositoryBase<Enrollment>(context), IEnrollmentRepository
{
    public async Task<IReadOnlyList<Enrollment>> GetByChildAsync(int childId, CancellationToken ct = default)
    {
        return await DbSet.AsNoTracking()
            .Include(e => e.KGPhase)
            .Include(e => e.AcademicYear)
            .Where(e => e.ChildId == childId)
            .OrderByDescending(e => e.EnrollmentDate)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Enrollment>> GetCurrentEnrollmentsAsync(CancellationToken ct = default)
    {
        return await DbSet.AsNoTracking()
            .Include(e => e.Child)
            .Include(e => e.KGPhase)
            .Include(e => e.AcademicYear)
            .Where(e => e.Status == EnrollmentStatus.Active && e.AcademicYear.IsActive)
            .ToListAsync(ct);
    }
}
