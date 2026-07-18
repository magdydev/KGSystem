using KGSystem.Domain.Entities;

namespace KGSystem.Domain.Repositories;

public interface IAcademicYearRepository : IRepository<AcademicYear>
{
    Task<AcademicYear?> GetActiveAsync(CancellationToken cancellationToken = default);
}
