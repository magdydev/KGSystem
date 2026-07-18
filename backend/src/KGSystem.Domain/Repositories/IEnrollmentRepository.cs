using KGSystem.Domain.Entities;

namespace KGSystem.Domain.Repositories;

public interface IEnrollmentRepository : IRepository<Enrollment>
{
    Task<IReadOnlyList<Enrollment>> GetByChildAsync(Guid childId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Enrollment>> GetCurrentEnrollmentsAsync(CancellationToken cancellationToken = default);
}
