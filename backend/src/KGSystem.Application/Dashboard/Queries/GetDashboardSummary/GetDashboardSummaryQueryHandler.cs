using KGSystem.Application.Common.Interfaces;
using KGSystem.Application.Dashboard.Dtos;
using KGSystem.Domain.Repositories;

namespace KGSystem.Application.Dashboard.Queries.GetDashboardSummary;

public sealed class GetDashboardSummaryQueryHandler(IUnitOfWork unitOfWork) : IQueryHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery query, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        var allChildren = await unitOfWork.Children.ListAllAsync(cancellationToken);
        var totalStudents = allChildren.Count;
        var activeStudents = allChildren.Count(c => c.Status == Domain.Enums.ChildStatus.Active);

        var dailyAbsent = await unitOfWork.Attendances.GetAbsentCountAsync(today, cancellationToken);
        var dailyIncome = await unitOfWork.Payments.GetDailyIncomeAsync(today, cancellationToken);

        var monthPayments = await unitOfWork.Payments.GetByDateRangeAsync(monthStart, monthEnd, cancellationToken);
        var monthlyIncome = monthPayments.Sum(p => p.AmountPaid);

        var pendingCount = await unitOfWork.Payments.GetPendingPaymentsCountAsync(cancellationToken);

        var pendingPayments = (await unitOfWork.Payments.ListAllAsync(cancellationToken))
            .Where(p => p.Status == Domain.Enums.PaymentStatus.Unpaid || p.Status == Domain.Enums.PaymentStatus.Partial)
            .ToList();
        var pendingAmount = pendingPayments.Sum(p => p.AmountDue - p.AmountPaid - p.Discount);

        var currentEnrollments = await unitOfWork.Enrollments.GetCurrentEnrollmentsAsync(cancellationToken);
        var phaseDistribution = currentEnrollments
            .GroupBy(e => e.KGPhaseId)
            .Select(g =>
            {
                var phase = g.First().KGPhase;
                return new PhaseDistributionItem(phase.NameAr, phase.NameEn, g.Count());
            })
            .ToList();

        var allPayments = await unitOfWork.Payments.ListAllAsync(cancellationToken);
        var paymentStatusDist = allPayments
            .GroupBy(p => p.Status)
            .Select(g => new PaymentStatusItem(g.Key.ToString(), g.Count(), g.Sum(p => p.AmountPaid)))
            .ToList();

        var recentPayments = allPayments
            .OrderByDescending(p => p.PaidDate ?? p.CreatedAt)
            .Take(10)
            .Select(p => new RecentPaymentItem(
                p.Id,
                p.Enrollment.Child.FirstNameAr + " " + p.Enrollment.Child.LastNameAr,
                p.Enrollment.Child.FirstNameEn + " " + p.Enrollment.Child.LastNameEn,
                p.AmountPaid,
                p.Status.ToString(),
                p.PaidDate))
            .ToList();

        var enrollmentTrends = currentEnrollments
            .GroupBy(e => e.EnrollmentDate.ToString("yyyy-MM"))
            .Select(g => new MonthlyEnrollmentTrend(g.Key, g.Count()))
            .OrderBy(t => t.Month)
            .ToList();

        var absentRate = activeStudents > 0 ? (decimal)dailyAbsent / activeStudents * 100 : 0;

        return new DashboardSummaryDto
        {
            TotalStudents = totalStudents,
            ActiveStudents = activeStudents,
            DailyAbsentCount = dailyAbsent,
            AbsentRate = Math.Round(absentRate, 1),
            DailyIncome = dailyIncome,
            MonthlyIncome = monthlyIncome,
            PendingPaymentsCount = pendingCount,
            PendingPaymentsAmount = pendingAmount,
            PhaseDistribution = phaseDistribution.AsReadOnly(),
            PaymentStatusDistribution = paymentStatusDist.AsReadOnly(),
            RecentPayments = recentPayments.AsReadOnly(),
            EnrollmentTrends = enrollmentTrends.AsReadOnly()
        };
    }
}
