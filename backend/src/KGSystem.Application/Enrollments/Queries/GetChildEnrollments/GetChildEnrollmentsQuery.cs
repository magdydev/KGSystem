using KGSystem.Application.Common.Interfaces;
using KGSystem.Application.Enrollments.Dtos;

namespace KGSystem.Application.Enrollments.Queries.GetChildEnrollments;

public sealed record GetChildEnrollmentsQuery(Guid ChildId) : IQuery<IReadOnlyList<EnrollmentDto>>;
