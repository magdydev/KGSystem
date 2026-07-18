namespace KGSystem.Application.Payments.Dtos;

public sealed record PaymentDto
{
    public Guid Id { get; init; }
    public Guid EnrollmentId { get; init; }
    public string ChildNameAr { get; init; } = string.Empty;
    public string ChildNameEn { get; init; } = string.Empty;
    public string KGPhaseNameAr { get; init; } = string.Empty;
    public string KGPhaseNameEn { get; init; } = string.Empty;
    public int Month { get; init; }
    public int Year { get; init; }
    public decimal AmountDue { get; init; }
    public decimal AmountPaid { get; init; }
    public decimal Discount { get; init; }
    public decimal Remaining => AmountDue - AmountPaid - Discount;
    public DateTime DueDate { get; init; }
    public DateTime? PaidDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Method { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public string? ReceivedBy { get; init; }
}
