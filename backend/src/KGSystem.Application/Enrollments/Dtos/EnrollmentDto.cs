namespace KGSystem.Application.Enrollments.Dtos;

public sealed record EnrollmentDto
{
    public int Id { get; init; }
    public int ChildId { get; init; }
    public int KGPhaseId { get; init; }
    public int AcademicYearId { get; init; }
    public string ChildNameAr { get; init; } = string.Empty;
    public string ChildNameEn { get; init; } = string.Empty;
    public string KGPhaseNameAr { get; init; } = string.Empty;
    public string KGPhaseNameEn { get; init; } = string.Empty;
    public string AcademicYearNameAr { get; init; } = string.Empty;
    public string AcademicYearNameEn { get; init; } = string.Empty;
    public DateTime EnrollmentDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? Notes { get; init; }
}
