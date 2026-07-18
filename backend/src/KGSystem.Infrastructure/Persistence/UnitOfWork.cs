using KGSystem.Domain.Repositories;
using KGSystem.Infrastructure.Persistence.Repositories;

namespace KGSystem.Infrastructure.Persistence;

public sealed class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private IChildRepository? _children;
    private IKGPhaseRepository? _kgPhases;
    private IAcademicYearRepository? _academicYears;
    private IEnrollmentRepository? _enrollments;
    private IMonthlyFeeRepository? _monthlyFees;
    private IPaymentRepository? _payments;
    private IAttendanceRepository? _attendances;
    private IBrandingRepository? _branding;

    public IChildRepository Children => _children ??= new ChildRepository(context);
    public IKGPhaseRepository KGPhases => _kgPhases ??= new KGPhaseRepository(context);
    public IAcademicYearRepository AcademicYears => _academicYears ??= new AcademicYearRepository(context);
    public IEnrollmentRepository Enrollments => _enrollments ??= new EnrollmentRepository(context);
    public IMonthlyFeeRepository MonthlyFees => _monthlyFees ??= new MonthlyFeeRepository(context);
    public IPaymentRepository Payments => _payments ??= new PaymentRepository(context);
    public IAttendanceRepository Attendances => _attendances ??= new AttendanceRepository(context);
    public IBrandingRepository Branding => _branding ??= new BrandingRepository(context);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
