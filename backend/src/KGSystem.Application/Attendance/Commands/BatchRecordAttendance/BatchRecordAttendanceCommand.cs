using KGSystem.Application.Common;
using KGSystem.Application.Common.Interfaces;

namespace KGSystem.Application.Attendance.Commands.BatchRecordAttendance;

public sealed record AttendanceRecord(int ChildId, string Status, string? Notes);

public sealed record BatchRecordAttendanceCommand(
    DateTime Date,
    IReadOnlyList<AttendanceRecord> Records) : ICommand<Unit>;
