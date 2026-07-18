using KGSystem.Domain.Common;
using KGSystem.Domain.Enums;
using KGSystem.Domain.Exceptions;

namespace KGSystem.Domain.Entities;

public sealed class Payment : BaseEntity, IAggregateRoot
{
    public Guid EnrollmentId { get; private set; }
    public int Month { get; private set; }
    public int Year { get; private set; }
    public decimal AmountDue { get; private set; }
    public decimal AmountPaid { get; private set; }
    public decimal Discount { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? PaidDate { get; private set; }
    public PaymentStatus Status { get; private set; } = PaymentStatus.Unpaid;
    public PaymentMethod Method { get; private set; }
    public string? Notes { get; private set; }
    public string? ReceivedBy { get; private set; }

    public Enrollment Enrollment { get; private set; } = null!;

    private Payment()
    {
    }

    public Payment(Guid enrollmentId, int month, int year, decimal amountDue, DateTime dueDate)
    {
        if (enrollmentId == Guid.Empty)
            throw new DomainException("Enrollment ID is required.");
        if (month < 1 || month > 12)
            throw new DomainException("Month must be between 1 and 12.");
        if (year < 2000)
            throw new DomainException("Invalid year.");
        if (amountDue < 0)
            throw new DomainException("Amount due cannot be negative.");
        if (dueDate == default)
            throw new DomainException("Due date is required.");

        EnrollmentId = enrollmentId;
        Month = month;
        Year = year;
        AmountDue = amountDue;
        DueDate = dueDate;
    }

    public void RecordPayment(decimal amountPaid, PaymentMethod method, string? receivedBy, DateTime? paidDate = null)
    {
        if (amountPaid <= 0)
            throw new DomainException("Payment amount must be greater than zero.");
        if (Status == PaymentStatus.Paid)
            throw new DomainException("Payment is already fully paid.");

        AmountPaid = amountPaid;
        Method = method;
        ReceivedBy = receivedBy?.Trim();
        PaidDate = paidDate ?? DateTime.UtcNow;

        Status = AmountPaid >= AmountDue ? PaymentStatus.Paid : PaymentStatus.Partial;
    }

    public void UpdatePayment(decimal amountPaid, decimal discount, PaymentMethod method, string? notes, string? receivedBy)
    {
        if (amountPaid < 0)
            throw new DomainException("Payment amount cannot be negative.");
        if (discount < 0)
            throw new DomainException("Discount cannot be negative.");

        AmountPaid = amountPaid;
        Discount = discount;
        Method = method;
        Notes = notes?.Trim();
        ReceivedBy = receivedBy?.Trim();

        var effectiveDue = AmountDue - Discount;
        Status = effectiveDue <= 0 || AmountPaid >= effectiveDue
            ? PaymentStatus.Paid
            : AmountPaid > 0 ? PaymentStatus.Partial : PaymentStatus.Unpaid;
    }
}
