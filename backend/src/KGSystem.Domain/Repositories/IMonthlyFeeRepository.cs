using KGSystem.Domain.Entities;

namespace KGSystem.Domain.Repositories;

public interface IMonthlyFeeRepository : IRepository<MonthlyFee>
{
    Task<IReadOnlyList<MonthlyFee>> GetByYearAsync(Guid yearId, CancellationToken cancellationToken = default);
    Task<MonthlyFee?> GetByYearAndMonthAsync(Guid yearId, int month, CancellationToken cancellationToken = default);
    Task RemoveByYearAsync(Guid yearId, CancellationToken cancellationToken = default);
    void RemoveRange(IEnumerable<MonthlyFee> fees);
}
