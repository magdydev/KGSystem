using KGSystem.Domain.Entities;
using KGSystem.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KGSystem.Infrastructure.Persistence.Repositories;

public sealed class MonthlyFeeRepository(ApplicationDbContext context) : RepositoryBase<MonthlyFee>(context), IMonthlyFeeRepository
{
    public override async Task<IReadOnlyList<MonthlyFee>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .Include(f => f.AcademicYear)
            .OrderBy(f => f.AcademicYear.Code)
            .ThenBy(f => f.Month)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MonthlyFee>> GetByYearAsync(Guid yearId, CancellationToken ct = default)
    {
        return await DbSet.AsNoTracking()
            .Include(f => f.AcademicYear)
            .Where(f => f.AcademicYearId == yearId)
            .OrderBy(f => f.Month)
            .ToListAsync(ct);
    }

    public async Task<MonthlyFee?> GetByYearAndMonthAsync(Guid yearId, int month, CancellationToken ct = default)
    {
        return await DbSet.AsNoTracking()
            .Include(f => f.AcademicYear)
            .FirstOrDefaultAsync(f => f.AcademicYearId == yearId && f.Month == month, ct);
    }

    public async Task RemoveByYearAsync(Guid yearId, CancellationToken ct = default)
    {
        await Context.Database.ExecuteSqlRawAsync(
            $"DELETE FROM MonthlyFees WHERE AcademicYearId = @p0",
            [yearId],
            ct);
    }

    public void RemoveRange(IEnumerable<MonthlyFee> fees)
    {
        DbSet.RemoveRange(fees);
    }
}
