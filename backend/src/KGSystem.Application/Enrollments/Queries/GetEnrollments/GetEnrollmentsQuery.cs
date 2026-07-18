using KGSystem.Application.Common.Interfaces;
using KGSystem.Application.Enrollments.Dtos;

namespace KGSystem.Application.Enrollments.Queries.GetEnrollments;

public sealed record GetEnrollmentsQuery : IQuery<IReadOnlyList<EnrollmentDto>>;
