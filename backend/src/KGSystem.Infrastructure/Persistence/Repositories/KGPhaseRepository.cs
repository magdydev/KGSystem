using KGSystem.Domain.Entities;
using KGSystem.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KGSystem.Infrastructure.Persistence.Repositories;

public sealed class KGPhaseRepository(ApplicationDbContext context) : RepositoryBase<KGPhase>(context), IKGPhaseRepository
{
    public async Task<IReadOnlyList<KGPhase>> GetOrderedAsync(CancellationToken ct = default)
    {
        return await DbSet.AsNoTracking()
            .OrderBy(p => p.SortOrder)
            .ToListAsync(ct);
    }
}
