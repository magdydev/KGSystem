using KGSystem.Domain.Common;
using KGSystem.Domain.Enums;
using KGSystem.Domain.Exceptions;

namespace KGSystem.Domain.Entities;

public sealed class Attendance : BaseEntity, IAggregateRoot
{
    public Guid ChildId { get; private set; }
    public DateTime Date { get; private set; }
    public AttendanceStatus Status { get; private set; }
    public string? Notes { get; private set; }

    public Child Child { get; private set; } = null!;

    private Attendance()
    {
    }

    public Attendance(Guid childId, DateTime date, AttendanceStatus status, string? notes = null)
    {
        if (childId == Guid.Empty)
            throw new DomainException("Child ID is required.");
        if (date == default)
            throw new DomainException("Date is required.");
        if (date > DateTime.UtcNow.Date)
            throw new DomainException("Attendance date cannot be in the future.");

        ChildId = childId;
        Date = date.Date;
        Status = status;
        Notes = notes?.Trim();
    }

    public void UpdateStatus(AttendanceStatus status, string? notes = null)
    {
        Status = status;
        Notes = notes?.Trim();
    }
}
