using KGSystem.Domain.Common;
using KGSystem.Domain.Enums;
using KGSystem.Domain.Exceptions;

namespace KGSystem.Domain.Entities;

public sealed class Enrollment : BaseEntity, IAggregateRoot
{
    public int ChildId { get; private set; }
    public int KGPhaseId { get; private set; }
    public int AcademicYearId { get; private set; }
    public DateTime EnrollmentDate { get; private set; }
    public EnrollmentStatus Status { get; private set; } = EnrollmentStatus.Active;
    public string? Notes { get; private set; }

    public Child Child { get; private set; } = null!;
    public KGPhase KGPhase { get; private set; } = null!;
    public AcademicYear AcademicYear { get; private set; } = null!;

    private Enrollment()
    {
    }

    public Enrollment(int childId, int kgPhaseId, int academicYearId, string? notes = null)
    {
        if (childId <= 0)
            throw new DomainException("Child ID is required.");
        if (kgPhaseId <= 0)
            throw new DomainException("KG phase ID is required.");
        if (academicYearId <= 0)
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
