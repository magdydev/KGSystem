using KGSystem.Domain.Common;
using KGSystem.Domain.Exceptions;
using KGSystem.Domain.ValueObjects;

namespace KGSystem.Domain.Entities;

public sealed class MonthlyFee : BaseEntity, IAggregateRoot
{
    public int AcademicYearId { get; private set; }
    public int Month { get; private set; }
    public Money Amount { get; private set; } = null!;
    public DateTime? DueDate { get; private set; }

    public AcademicYear AcademicYear { get; private set; } = null!;

    private MonthlyFee()
    {
    }

    public MonthlyFee(int academicYearId, int month, Money amount, DateTime? dueDate = null)
    {
        if (academicYearId <= 0)
            throw new DomainException("Academic year ID is required.");
        if (month < 1 || month > 12)
            throw new DomainException("Month must be between 1 and 12.");
        if (amount is null)
            throw new DomainException("Amount is required.");

        AcademicYearId = academicYearId;
        Month = month;
        Amount = amount;
        DueDate = dueDate;
    }

    public void Update(Money amount, DateTime? dueDate)
    {
        if (amount is null)
            throw new DomainException("Amount is required.");

        Amount = amount;
        DueDate = dueDate;
    }
}
