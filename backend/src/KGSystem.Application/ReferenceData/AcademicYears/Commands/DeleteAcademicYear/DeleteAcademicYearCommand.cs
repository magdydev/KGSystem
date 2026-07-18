using KGSystem.Application.Common;
using KGSystem.Application.Common.Interfaces;

namespace KGSystem.Application.ReferenceData.AcademicYears.Commands.DeleteAcademicYear;

public sealed record DeleteAcademicYearCommand(int Id) : ICommand<Unit>;
