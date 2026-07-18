namespace KGSystem.Application.ReferenceData.MonthlyFees.Dtos;

public sealed record MonthlyFeeDto
{
    public Guid Id { get; init; }
    public Guid AcademicYearId { get; init; }
    public string AcademicYearNameAr { get; init; } = string.Empty;
    public string AcademicYearNameEn { get; init; } = string.Empty;
    public int Month { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public DateTime? DueDate { get; init; }
}
