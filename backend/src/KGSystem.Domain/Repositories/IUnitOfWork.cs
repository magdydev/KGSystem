namespace KGSystem.Domain.Repositories;

public interface IUnitOfWork
{
    IChildRepository Children { get; }
    IKGPhaseRepository KGPhases { get; }
    IAcademicYearRepository AcademicYears { get; }
    IEnrollmentRepository Enrollments { get; }
    IMonthlyFeeRepository MonthlyFees { get; }
    IPaymentRepository Payments { get; }
    IAttendanceRepository Attendances { get; }
    IBrandingRepository Branding { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
