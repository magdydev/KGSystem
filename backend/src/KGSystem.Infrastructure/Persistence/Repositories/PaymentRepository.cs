using KGSystem.Domain.Entities;
using KGSystem.Domain.Enums;
using KGSystem.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KGSystem.Infrastructure.Persistence.Repositories;

public sealed class PaymentRepository(ApplicationDbContext context) : RepositoryBase<Payment>(context), IPaymentRepository
{
    public override async Task<IReadOnlyList<Payment>> ListAllAsync(CancellationToken ct = default)
    {
        return await DbSet.AsNoTracking()
            .Include(p => p.Enrollment).ThenInclude(e => e.Child)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Payment>> GetByEnrollmentAsync(Guid enrollmentId, CancellationToken ct = default)
    {
        return await DbSet.AsNoTracking()
            .Where(p => p.EnrollmentId == enrollmentId)
            .OrderByDescending(p => p.Year).ThenByDescending(p => p.Month)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Payment>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        return await DbSet.AsNoTracking()
            .Include(p => p.Enrollment).ThenInclude(e => e.Child)
            .Where(p => p.PaidDate >= from && p.PaidDate <= to)
            .OrderByDescending(p => p.PaidDate)
            .ToListAsync(ct);
    }

    public async Task<decimal> GetDailyIncomeAsync(DateTime date, CancellationToken ct = default)
    {
        var start = date.Date;
        var end = start.AddDays(1);

        return await DbSet.AsNoTracking()
            .Where(p => p.PaidDate >= start && p.PaidDate < end && p.Status == PaymentStatus.Paid)
            .SumAsync(p => p.AmountPaid, ct);
    }

    public async Task<int> GetPendingPaymentsCountAsync(CancellationToken ct = default)
    {
        return await DbSet.AsNoTracking()
            .Where(p => p.Status == PaymentStatus.Unpaid || p.Status == PaymentStatus.Partial)
            .CountAsync(ct);
    }
}
