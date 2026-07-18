using KGSystem.Domain.Common;
using KGSystem.Domain.Enums;
using KGSystem.Domain.Exceptions;

namespace KGSystem.Domain.Entities;

public sealed class Child : BaseEntity, IAggregateRoot
{
    public string FirstNameAr { get; private set; }
    public string FirstNameEn { get; private set; }
    public string LastNameAr { get; private set; }
    public string LastNameEn { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public Gender Gender { get; private set; }
    public string? Nationality { get; private set; }
    public string GuardianNameAr { get; private set; }
    public string GuardianNameEn { get; private set; }
    public string GuardianPhone { get; private set; }
    public string? GuardianEmail { get; private set; }
    public string? Address { get; private set; }
    public string? PhotoUrl { get; private set; }
    public ChildStatus Status { get; private set; } = ChildStatus.Active;
    public DateTime EnrollmentDate { get; private set; }

    private readonly List<Enrollment> _enrollments = [];
    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

    private readonly List<Attendance> _attendances = [];
    public IReadOnlyCollection<Attendance> Attendances => _attendances.AsReadOnly();

    private Child()
    {
        FirstNameAr = null!;
        FirstNameEn = null!;
        LastNameAr = null!;
        LastNameEn = null!;
        GuardianNameAr = null!;
        GuardianNameEn = null!;
        GuardianPhone = null!;
    }

    public Child(
        string firstNameAr,
        string firstNameEn,
        string lastNameAr,
        string lastNameEn,
        DateTime dateOfBirth,
        Gender gender,
        string guardianNameAr,
        string guardianNameEn,
        string guardianPhone,
        string? guardianEmail = null,
        string? nationality = null,
        string? address = null)
    {
        if (string.IsNullOrWhiteSpace(firstNameAr))
            throw new DomainException("Arabic first name is required.");
        if (string.IsNullOrWhiteSpace(firstNameEn))
            throw new DomainException("English first name is required.");
        if (string.IsNullOrWhiteSpace(lastNameAr))
            throw new DomainException("Arabic last name is required.");
        if (string.IsNullOrWhiteSpace(lastNameEn))
            throw new DomainException("English last name is required.");
        if (dateOfBirth == default)
            throw new DomainException("Date of birth is required.");
        if (dateOfBirth > DateTime.UtcNow)
            throw new DomainException("Date of birth cannot be in the future.");
        if (string.IsNullOrWhiteSpace(guardianNameAr))
            throw new DomainException("Arabic guardian name is required.");
        if (string.IsNullOrWhiteSpace(guardianNameEn))
            throw new DomainException("English guardian name is required.");
        if (string.IsNullOrWhiteSpace(guardianPhone))
            throw new DomainException("Guardian phone is required.");

        FirstNameAr = firstNameAr.Trim();
        FirstNameEn = firstNameEn.Trim();
        LastNameAr = lastNameAr.Trim();
        LastNameEn = lastNameEn.Trim();
        DateOfBirth = dateOfBirth;
        Gender = gender;
        GuardianNameAr = guardianNameAr.Trim();
        GuardianNameEn = guardianNameEn.Trim();
        GuardianPhone = guardianPhone.Trim();
        GuardianEmail = guardianEmail?.Trim();
        Nationality = nationality?.Trim();
        Address = address?.Trim();
        EnrollmentDate = DateTime.UtcNow;
    }

    public void UpdatePersonalInfo(
        string firstNameAr,
        string firstNameEn,
        string lastNameAr,
        string lastNameEn,
        DateTime dateOfBirth,
        Gender gender,
        string? nationality,
        string? address)
    {
        if (string.IsNullOrWhiteSpace(firstNameAr))
            throw new DomainException("Arabic first name is required.");
        if (string.IsNullOrWhiteSpace(firstNameEn))
            throw new DomainException("English first name is required.");
        if (string.IsNullOrWhiteSpace(lastNameAr))
            throw new DomainException("Arabic last name is required.");
        if (string.IsNullOrWhiteSpace(lastNameEn))
            throw new DomainException("English last name is required.");
        if (dateOfBirth == default)
            throw new DomainException("Date of birth is required.");
        if (dateOfBirth > DateTime.UtcNow)
            throw new DomainException("Date of birth cannot be in the future.");

        FirstNameAr = firstNameAr.Trim();
        FirstNameEn = firstNameEn.Trim();
        LastNameAr = lastNameAr.Trim();
        LastNameEn = lastNameEn.Trim();
        DateOfBirth = dateOfBirth;
        Gender = gender;
        Nationality = nationality?.Trim();
        Address = address?.Trim();
    }

    public void UpdateGuardianInfo(string guardianNameAr, string guardianNameEn, string guardianPhone, string? guardianEmail)
    {
        if (string.IsNullOrWhiteSpace(guardianNameAr))
            throw new DomainException("Arabic guardian name is required.");
        if (string.IsNullOrWhiteSpace(guardianNameEn))
            throw new DomainException("English guardian name is required.");
        if (string.IsNullOrWhiteSpace(guardianPhone))
            throw new DomainException("Guardian phone is required.");

        GuardianNameAr = guardianNameAr.Trim();
        GuardianNameEn = guardianNameEn.Trim();
        GuardianPhone = guardianPhone.Trim();
        GuardianEmail = guardianEmail?.Trim();
    }

    public void UpdateStatus(ChildStatus status)
    {
        Status = status;
    }

    public void UpdatePhoto(string photoUrl)
    {
        if (string.IsNullOrWhiteSpace(photoUrl))
            throw new DomainException("Photo URL is required.");

        PhotoUrl = photoUrl.Trim();
    }
}
