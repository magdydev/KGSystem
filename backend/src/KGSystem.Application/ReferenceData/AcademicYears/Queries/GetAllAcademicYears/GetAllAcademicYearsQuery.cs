using KGSystem.Application.Common.Interfaces;
using KGSystem.Application.ReferenceData.AcademicYears.Dtos;

namespace KGSystem.Application.ReferenceData.AcademicYears.Queries.GetAllAcademicYears;

public sealed record GetAllAcademicYearsQuery : IQuery<IReadOnlyList<AcademicYearDto>>;
