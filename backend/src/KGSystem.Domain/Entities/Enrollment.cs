using KGSystem.Domain.Common;
using KGSystem.Domain.Enums;
using KGSystem.Domain.Exceptions;

namespace KGSystem.Domain.Entities;

public sealed class Enrollment : BaseEntity, IAggregateRoot
{
    public Guid ChildId { get; private set; }
    public Guid KGPhaseId { get; private set; }
    public Guid AcademicYearId { get; private set; }
    public DateTime EnrollmentDate { get; private set; }
    public EnrollmentStatus Status { get; private set; } = EnrollmentStatus.Active;
    public string? Notes { get; private set; }

    public Child Child { get; private set; } = null!;
    public KGPhase KGPhase { get; private set; } = null!;
    public AcademicYear AcademicYear { get; private set; } = null!;

    private Enrollment()
    {
    }

    public Enrollment(Guid childId, Guid kgPhaseId, Guid academicYearId, string? notes = null)
    {
        if (childId == Guid.Empty)
            throw new DomainException("Child ID is required.");
        if (kgPhaseId == Guid.Empty)
            throw new DomainException("KG phase ID is required.");
        if (academicYearId == Guid.Empty)
            throw new DomainException("Academic year ID is required.");

        ChildId = childId;
        KGPhaseId = kgPhaseId;
        AcademicYearId = academicYearId;
        EnrollmentDate = DateTime.UtcNow;
        Notes = notes?.Trim();
    }

    public void Withdraw()
    {
        if (Status == EnrollmentStatus.Withdrawn)
            throw new DomainException("Enrollment is already withdrawn.");

        Status = EnrollmentStatus.Withdrawn;
    }

    public void Complete()
    {
        if (Status == EnrollmentStatus.Completed)
            throw new DomainException("Enrollment is already completed.");

        Status = EnrollmentStatus.Completed;
    }

    public void UpdateNotes(string notes)
    {
        Notes = notes?.Trim();
    }
}
