using KGSystem.Application.Common.Interfaces;

namespace KGSystem.Application.Enrollments.Commands.CreateEnrollment;

public sealed record CreateEnrollmentCommand(
    int ChildId,
    int KGPhaseId,
    int AcademicYearId,
    string? Notes) : ICommand<int>;
