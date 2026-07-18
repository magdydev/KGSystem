using KGSystem.Application.Common.Interfaces;

namespace KGSystem.Application.Enrollments.Commands.CreateEnrollment;

public sealed record CreateEnrollmentCommand(
    Guid ChildId,
    Guid KGPhaseId,
    Guid AcademicYearId,
    string? Notes) : ICommand<Guid>;
