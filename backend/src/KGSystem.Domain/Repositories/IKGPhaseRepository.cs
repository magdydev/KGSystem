using KGSystem.Domain.Entities;

namespace KGSystem.Domain.Repositories;

public interface IKGPhaseRepository : IRepository<KGPhase>
{
    Task<IReadOnlyList<KGPhase>> GetOrderedAsync(CancellationToken cancellationToken = default);
}
