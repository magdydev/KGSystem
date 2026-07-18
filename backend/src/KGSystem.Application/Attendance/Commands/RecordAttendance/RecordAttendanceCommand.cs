using KGSystem.Application.Common.Interfaces;

namespace KGSystem.Application.Attendance.Commands.RecordAttendance;

public sealed record RecordAttendanceCommand(
    Guid ChildId,
    DateTime Date,
    string Status,
    string? Notes) : ICommand<Guid>;
