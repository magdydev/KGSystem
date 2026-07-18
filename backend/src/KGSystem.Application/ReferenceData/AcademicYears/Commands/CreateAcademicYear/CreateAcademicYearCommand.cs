using KGSystem.Application.Common.Interfaces;

namespace KGSystem.Application.ReferenceData.AcademicYears.Commands.CreateAcademicYear;

public sealed record CreateAcademicYearCommand(
    string Code,
    string NameAr,
    string NameEn,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive) : ICommand<Guid>;
