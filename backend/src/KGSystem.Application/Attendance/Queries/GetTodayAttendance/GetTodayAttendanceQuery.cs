using KGSystem.Application.Attendance.Dtos;
using KGSystem.Application.Common.Interfaces;

namespace KGSystem.Application.Attendance.Queries.GetTodayAttendance;

public sealed record GetTodayAttendanceQuery : IQuery<IReadOnlyList<AttendanceDto>>;
