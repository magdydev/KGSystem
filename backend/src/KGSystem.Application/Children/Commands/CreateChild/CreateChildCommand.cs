using KGSystem.Application.Common.Interfaces;

namespace KGSystem.Application.Children.Commands.CreateChild;

public sealed record CreateChildCommand(
    string FirstNameAr,
    string FirstNameEn,
    string LastNameAr,
    string LastNameEn,
    DateTime DateOfBirth,
    string Gender,
    string GuardianNameAr,
    string GuardianNameEn,
    string GuardianPhone,
    string? GuardianEmail,
    string? Nationality,
    string? Address,
    Guid? KGPhaseId,
    Guid? AcademicYearId) : ICommand<Guid>;
