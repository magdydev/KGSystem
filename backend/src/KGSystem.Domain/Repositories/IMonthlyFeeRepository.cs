using KGSystem.Domain.Entities;

namespace KGSystem.Domain.Repositories;

public interface IMonthlyFeeRepository : IRepository<MonthlyFee>
{
    Task<IReadOnlyList<MonthlyFee>> GetByYearAsync(int yearId, CancellationToken cancellationToken = default);
    Task<MonthlyFee?> GetByYearAndMonthAsync(int yearId, int month, CancellationToken cancellationToken = default);
    Task RemoveByYearAsync(int yearId, CancellationToken cancellationToken = default);
    void RemoveRange(IEnumerable<MonthlyFee> fees);
}
