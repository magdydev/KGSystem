using KGSystem.Domain.Entities;
using KGSystem.Domain.Enums;
using KGSystem.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KGSystem.Infrastructure.Persistence.Repositories;

public sealed class ChildRepository(ApplicationDbContext context) : RepositoryBase<Child>(context), IChildRepository
{
    public async Task<IReadOnlyList<Child>> SearchAsync(string? name, ChildStatus? status, Guid? phaseId, CancellationToken ct = default)
    {
        var query = DbSet.AsNoTracking()
            .Include(c => c.Enrollments).ThenInclude(e => e.KGPhase)
            .Include(c => c.Enrollments).ThenInclude(e => e.AcademicYear)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status.Value);
        }

        if (phaseId.HasValue)
        {
            query = query.Where(c => c.Enrollments.Any(e => e.KGPhaseId == phaseId.Value && e.Status == EnrollmentStatus.Active));
        }

        var children = await query.ToListAsync(ct);

        if (!string.IsNullOrWhiteSpace(name))
        {
            var normalized = NormalizeArabic(name.Trim().ToLower());
            children = children.Where(c =>
                NormalizeArabic(c.FirstNameAr?.ToLower() ?? "").Contains(normalized) ||
                NormalizeArabic(c.FirstNameEn?.ToLower() ?? "").Contains(normalized) ||
                NormalizeArabic(c.LastNameAr?.ToLower() ?? "").Contains(normalized) ||
                NormalizeArabic(c.LastNameEn?.ToLower() ?? "").Contains(normalized)
            ).ToList();
        }

        return children;
    }

    private static string NormalizeArabic(string text)
    {
        return text
            .Replace('أ', 'ا')
            .Replace('إ', 'ا')
            .Replace('آ', 'ا')
            .Replace('ة', 'ه')
            .Replace('ى', 'ي')
            .Replace('ؤ', 'و')
            .Replace('ئ', 'ي');
    }

    public async Task<Child?> GetWithEnrollmentsAsync(Guid id, CancellationToken ct = default)
    {
        return await DbSet
            .Include(c => c.Enrollments.OrderByDescending(e => e.AcademicYear.StartDate)).ThenInclude(e => e.KGPhase)
            .Include(c => c.Enrollments).ThenInclude(e => e.AcademicYear)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }
}
