using KGSystem.Domain.Entities;
using KGSystem.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KGSystem.Infrastructure.Persistence.Repositories;

public sealed class AcademicYearRepository(ApplicationDbContext context) : RepositoryBase<AcademicYear>(context), IAcademicYearRepository
{
    public async Task<AcademicYear?> GetActiveAsync(CancellationToken ct = default)
    {
        return await DbSet.AsNoTracking()
            .FirstOrDefaultAsync(y => y.IsActive, ct);
    }
}
