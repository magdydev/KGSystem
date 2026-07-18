namespace KGSystem.Application.Dashboard.Dtos;

public sealed record DashboardSummaryDto
{
    public int TotalStudents { get; init; }
    public int ActiveStudents { get; init; }
    public int DailyAbsentCount { get; init; }
    public decimal AbsentRate { get; init; }
    public decimal DailyIncome { get; init; }
    public decimal MonthlyIncome { get; init; }
    public int PendingPaymentsCount { get; init; }
    public decimal PendingPaymentsAmount { get; init; }
    public IReadOnlyList<PhaseDistributionItem> PhaseDistribution { get; init; } = [];
    public IReadOnlyList<PaymentStatusItem> PaymentStatusDistribution { get; init; } = [];
    public IReadOnlyList<RecentPaymentItem> RecentPayments { get; init; } = [];
    public IReadOnlyList<MonthlyEnrollmentTrend> EnrollmentTrends { get; init; } = [];
}

public sealed record PhaseDistributionItem(string PhaseNameAr, string PhaseNameEn, int StudentCount);

public sealed record PaymentStatusItem(string Status, int Count, decimal Amount);

public sealed record RecentPaymentItem(Guid Id, string ChildNameAr, string ChildNameEn, decimal Amount, string Status, DateTime? PaidDate);

public sealed record MonthlyEnrollmentTrend(string Month, int Count);
