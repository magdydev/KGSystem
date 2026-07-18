using KGSystem.Application.Common;
using KGSystem.Application.Common.Interfaces;

namespace KGSystem.Application.Enrollments.Commands.UpdateEnrollment;

public sealed record UpdateEnrollmentCommand(
    int Id,
    string Status,
    string? Notes) : ICommand<Unit>;
