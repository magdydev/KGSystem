using KGSystem.Domain.Entities;
using KGSystem.Domain.Enums;
using KGSystem.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KGSystem.Infrastructure.Persistence.Repositories;

public sealed class AttendanceRepository(ApplicationDbContext context) : RepositoryBase<Attendance>(context), IAttendanceRepository
{
    public override async Task<IReadOnlyList<Attendance>> ListAllAsync(CancellationToken ct = default) =>
        await DbSet.AsNoTracking().Include(a => a.Child).ToListAsync(ct);

    public async Task<IReadOnlyList<Attendance>> GetByDateAsync(DateTime date, CancellationToken ct = default)
    {
        return await DbSet.AsNoTracking()
            .Include(a => a.Child)
            .Where(a => a.Date == date.Date)
            .OrderBy(a => a.Child.FirstNameEn)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Attendance>> GetByChildAsync(int childId, CancellationToken ct = default)
    {
        return await DbSet.Where(a => a.ChildId == childId).ToListAsync(ct);
    }

    public async Task<int> GetAbsentCountAsync(DateTime date, CancellationToken ct = default)
    {
        return await DbSet.AsNoTracking()
            .Where(a => a.Date == date.Date && a.Status == AttendanceStatus.Absent)
            .CountAsync(ct);
    }

    public async Task<Attendance?> GetByChildAndDateAsync(int childId, DateTime date, CancellationToken ct = default)
    {
        return await DbSet.AsNoTracking()
            .FirstOrDefaultAsync(a => a.ChildId == childId && a.Date == date.Date, ct);
    }

    public async Task RemoveByChildIdsAndDateAsync(IEnumerable<int> childIds, DateTime date, CancellationToken ct = default)
    {
        var ids = childIds.ToList();
        if (ids.Count == 0) return;

        var placeholders = string.Join(",", ids.Select((_, i) => $"@p{i + 1}"));
        var parameters = new object[] { date.Date }.Concat(ids.Cast<object>()).ToArray();
        await Context.Database.ExecuteSqlRawAsync(
            $"DELETE FROM Attendances WHERE Date = @p0 AND ChildId IN ({placeholders})",
            parameters,
            ct);
    }
}
