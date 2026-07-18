namespace KGSystem.Application.Attendance.Dtos;

public sealed record AttendanceDto
{
    public Guid Id { get; init; }
    public Guid ChildId { get; init; }
    public string ChildNameAr { get; init; } = string.Empty;
    public string ChildNameEn { get; init; } = string.Empty;
    public DateTime Date { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? Notes { get; init; }
}
