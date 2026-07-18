using AutoMapper;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Application.Enrollments.Dtos;
using KGSystem.Domain.Repositories;

namespace KGSystem.Application.Enrollments.Queries.GetEnrollments;

public sealed class GetEnrollmentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IQueryHandler<GetEnrollmentsQuery, IReadOnlyList<EnrollmentDto>>
{
    public async Task<IReadOnlyList<EnrollmentDto>> Handle(GetEnrollmentsQuery query, CancellationToken cancellationToken)
    {
        var enrollments = await unitOfWork.Enrollments.ListAllAsync(cancellationToken);

        return mapper.Map<IReadOnlyList<EnrollmentDto>>(enrollments);
    }
}
