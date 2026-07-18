using KGSystem.Application.Common;
using KGSystem.Application.Common.Interfaces;

namespace KGSystem.Application.ReferenceData.AcademicYears.Commands.UpdateAcademicYear;

public sealed record UpdateAcademicYearCommand(
    int Id,
    string NameAr,
    string NameEn,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive) : ICommand<Unit>;
