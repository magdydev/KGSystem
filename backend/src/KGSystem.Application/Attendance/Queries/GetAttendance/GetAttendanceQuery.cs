using KGSystem.Application.Attendance.Dtos;
using KGSystem.Application.Common.Interfaces;

namespace KGSystem.Application.Attendance.Queries.GetAttendance;

public sealed record GetAttendanceQuery(
    DateTime? Date,
    Guid? ChildId,
    string? Status) : IQuery<IReadOnlyList<AttendanceDto>>;
