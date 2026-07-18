using KGSystem.Application.Common.Interfaces;
using KGSystem.Application.ReferenceData.AcademicYears.Dtos;

namespace KGSystem.Application.ReferenceData.AcademicYears.Queries.GetArchivedYearDetail;

public sealed record GetArchivedYearDetailQuery(int Id) : IQuery<ArchivedYearDetailDto>;
