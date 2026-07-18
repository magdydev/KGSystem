namespace KGSystem.Application.ReferenceData.MonthlyFees.Dtos;

public sealed record MonthlyFeeDto
{
    public int Id { get; init; }
    public int AcademicYearId { get; init; }
    public string AcademicYearNameAr { get; init; } = string.Empty;
    public string AcademicYearNameEn { get; init; } = string.Empty;
    public int Month { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public DateTime? DueDate { get; init; }
}
