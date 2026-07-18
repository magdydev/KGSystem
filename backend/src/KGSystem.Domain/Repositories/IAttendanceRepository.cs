using KGSystem.Domain.Entities;

namespace KGSystem.Domain.Repositories;

public interface IAttendanceRepository : IRepository<Attendance>
{
    Task<IReadOnlyList<Attendance>> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Attendance>> GetByChildAsync(int childId, CancellationToken cancellationToken = default);
    Task<int> GetAbsentCountAsync(DateTime date, CancellationToken cancellationToken = default);
    Task<Attendance?> GetByChildAndDateAsync(int childId, DateTime date, CancellationToken cancellationToken = default);
    Task RemoveByChildIdsAndDateAsync(IEnumerable<int> childIds, DateTime date, CancellationToken cancellationToken = default);
}
