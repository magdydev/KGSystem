using AutoMapper;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Application.Enrollments.Dtos;
using KGSystem.Domain.Repositories;

namespace KGSystem.Application.Enrollments.Queries.GetChildEnrollments;

public sealed class GetChildEnrollmentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IQueryHandler<GetChildEnrollmentsQuery, IReadOnlyList<EnrollmentDto>>
{
    public async Task<IReadOnlyList<EnrollmentDto>> Handle(GetChildEnrollmentsQuery query, CancellationToken cancellationToken)
    {
        var enrollments = await unitOfWork.Enrollments.GetByChildAsync(query.ChildId, cancellationToken);

        return mapper.Map<IReadOnlyList<EnrollmentDto>>(enrollments);
    }
}
