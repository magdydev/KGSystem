using KGSystem.Domain.Common;
using KGSystem.Domain.Exceptions;

namespace KGSystem.Domain.Entities;

public sealed class AcademicYear : BaseEntity, IAggregateRoot
{
    public string Code { get; private set; }
    public string NameAr { get; private set; }
    public string NameEn { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<Enrollment> _enrollments = [];
    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

    private readonly List<MonthlyFee> _monthlyFees = [];
    public IReadOnlyCollection<MonthlyFee> MonthlyFees => _monthlyFees.AsReadOnly();

    private AcademicYear()
    {
        Code = null!;
        NameAr = null!;
        NameEn = null!;
    }

    public AcademicYear(string code, string nameAr, string nameEn, DateTime startDate, DateTime endDate, bool isActive = false)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Academic year code is required.");
        if (string.IsNullOrWhiteSpace(nameAr))
            throw new DomainException("Arabic academic year name is required.");
        if (string.IsNullOrWhiteSpace(nameEn))
            throw new DomainException("English academic year name is required.");
        if (startDate == default)
            throw new DomainException("Start date is required.");
        if (endDate == default)
            throw new DomainException("End date is required.");
        if (endDate <= startDate)
            throw new DomainException("End date must be after start date.");

        Code = code.Trim();
        NameAr = nameAr.Trim();
        NameEn = nameEn.Trim();
        StartDate = startDate;
        EndDate = endDate;
        IsActive = isActive;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Update(string nameAr, string nameEn, DateTime startDate, DateTime endDate)
    {
        if (string.IsNullOrWhiteSpace(nameAr))
            throw new DomainException("Arabic academic year name is required.");
        if (string.IsNullOrWhiteSpace(nameEn))
            throw new DomainException("English academic year name is required.");
        if (startDate == default)
            throw new DomainException("Start date is required.");
        if (endDate == default)
            throw new DomainException("End date is required.");
        if (endDate <= startDate)
            throw new DomainException("End date must be after start date.");

        NameAr = nameAr.Trim();
        NameEn = nameEn.Trim();
        StartDate = startDate;
        EndDate = endDate;
    }
}
