using KGSystem.Application.Common;
using KGSystem.Application.Common.Interfaces;

namespace KGSystem.Application.Children.Commands.UpdateChild;

public sealed record UpdateChildCommand(
    int Id,
    string FirstNameAr,
    string FirstNameEn,
    string LastNameAr,
    string LastNameEn,
    DateTime DateOfBirth,
    string Gender,
    string? Nationality,
    string? Address,
    string GuardianNameAr,
    string GuardianNameEn,
    string GuardianPhone,
    string? GuardianEmail) : ICommand<Unit>;
