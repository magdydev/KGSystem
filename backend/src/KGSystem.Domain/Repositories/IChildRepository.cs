using KGSystem.Domain.Entities;
using KGSystem.Domain.Enums;

namespace KGSystem.Domain.Repositories;

public interface IChildRepository : IRepository<Child>
{
    Task<IReadOnlyList<Child>> SearchAsync(string? name, ChildStatus? status, Guid? phaseId, CancellationToken cancellationToken = default);
    Task<Child?> GetWithEnrollmentsAsync(Guid id, CancellationToken cancellationToken = default);
}
