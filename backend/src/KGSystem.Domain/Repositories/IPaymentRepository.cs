using KGSystem.Domain.Entities;

namespace KGSystem.Domain.Repositories;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<IReadOnlyList<Payment>> GetByEnrollmentAsync(Guid enrollmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Payment>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<decimal> GetDailyIncomeAsync(DateTime date, CancellationToken cancellationToken = default);
    Task<int> GetPendingPaymentsCountAsync(CancellationToken cancellationToken = default);
}
