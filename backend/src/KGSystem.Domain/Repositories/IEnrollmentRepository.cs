using KGSystem.Domain.Entities;

namespace KGSystem.Domain.Repositories;

public interface IEnrollmentRepository : IRepository<Enrollment>
{
    Task<IReadOnlyList<Enrollment>> GetByChildAsync(int childId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Enrollment>> GetCurrentEnrollmentsAsync(CancellationToken cancellationToken = default);
}
