using KGSystem.Domain.Entities;

namespace KGSystem.Domain.Repositories;

public interface IBrandingRepository : IRepository<BrandingSetting>
{
    Task<BrandingSetting?> GetDefaultAsync(CancellationToken cancellationToken = default);
    Task<string?> GetCurrencyAsync(CancellationToken cancellationToken = default);
}
