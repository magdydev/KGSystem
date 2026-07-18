using KGSystem.Application.Enrollments.Dtos;

namespace KGSystem.Application.Children.Dtos;

public sealed record ChildDetailDto
{
    public int Id { get; init; }
    public string FirstNameAr { get; init; } = string.Empty;
    public string FirstNameEn { get; init; } = string.Empty;
    public string LastNameAr { get; init; } = string.Empty;
    public string LastNameEn { get; init; } = string.Empty;
    public string FullNameAr => $"{FirstNameAr} {LastNameAr}";
    public string FullNameEn => $"{FirstNameEn} {LastNameEn}";
    public DateTime DateOfBirth { get; init; }
    public string Gender { get; init; } = string.Empty;
    public string? Nationality { get; init; }
    public string GuardianNameAr { get; init; } = string.Empty;
    public string GuardianNameEn { get; init; } = string.Empty;
    public string GuardianPhone { get; init; } = string.Empty;
    public string? GuardianEmail { get; init; }
    public string? Address { get; init; }
    public string? PhotoUrl { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime EnrollmentDate { get; init; }
    public DateTime CreatedAt { get; init; }
    public List<EnrollmentDto> Enrollments { get; init; } = [];
}
