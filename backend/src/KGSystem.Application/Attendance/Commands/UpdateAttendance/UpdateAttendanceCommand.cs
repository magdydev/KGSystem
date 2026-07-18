using KGSystem.Application.Common;
using KGSystem.Application.Common.Interfaces;

namespace KGSystem.Application.Attendance.Commands.UpdateAttendance;

public sealed record UpdateAttendanceCommand(
    int Id,
    string Status,
    string? Notes) : ICommand<Unit>;
