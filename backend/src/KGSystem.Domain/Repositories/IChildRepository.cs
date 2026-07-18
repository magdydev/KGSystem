using KGSystem.Domain.Entities;
using KGSystem.Domain.Enums;

namespace KGSystem.Domain.Repositories;

public interface IChildRepository : IRepository<Child>
{
    Task<IReadOnlyList<Child>> SearchAsync(string? name, ChildStatus? status, int? phaseId, CancellationToken cancellationToken = default);
    Task<Child?> GetWithEnrollmentsAsync(int id, CancellationToken cancellationToken = default);
}
