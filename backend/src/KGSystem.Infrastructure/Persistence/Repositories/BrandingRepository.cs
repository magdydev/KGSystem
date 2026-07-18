using KGSystem.Domain.Entities;
using KGSystem.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KGSystem.Infrastructure.Persistence.Repositories;

public sealed class BrandingRepository(ApplicationDbContext context) : RepositoryBase<BrandingSetting>(context), IBrandingRepository
{
    public async Task<BrandingSetting?> GetDefaultAsync(CancellationToken ct = default)
    {
        return await DbSet.AsNoTracking().FirstOrDefaultAsync(ct);
    }

    public async Task<string?> GetCurrencyAsync(CancellationToken ct = default)
    {
        return await DbSet.AsNoTracking().Select(b => b.Currency).FirstOrDefaultAsync(ct);
    }
}
