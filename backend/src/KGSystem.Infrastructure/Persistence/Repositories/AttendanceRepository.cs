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

    public async Task<int> GetAbsentCountAsync(DateTime date, CancellationToken ct = default)
    {
        return await DbSet.AsNoTracking()
            .Where(a => a.Date == date.Date && a.Status == AttendanceStatus.Absent)
            .CountAsync(ct);
    }

    public async Task<Attendance?> GetByChildAndDateAsync(Guid childId, DateTime date, CancellationToken ct = default)
    {
        return await DbSet.AsNoTracking()
            .FirstOrDefaultAsync(a => a.ChildId == childId && a.Date == date.Date, ct);
    }

    public async Task RemoveByChildIdsAndDateAsync(IEnumerable<Guid> childIds, DateTime date, CancellationToken ct = default)
    {
        var ids = childIds.ToList();
        if (ids.Count == 0) return;

        var idList = string.Join(",", ids.Select(id => $"'{id}'"));
        await Context.Database.ExecuteSqlRawAsync(
            $"DELETE FROM Attendances WHERE Date = @p0 AND ChildId IN ({idList})",
            [date.Date],
            ct);
    }
}
