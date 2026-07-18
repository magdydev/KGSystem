using AutoMapper;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Application.ReferenceData.AcademicYears.Dtos;
using KGSystem.Domain.Repositories;

namespace KGSystem.Application.ReferenceData.AcademicYears.Queries.GetAllAcademicYears;

public sealed class GetAllAcademicYearsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IQueryHandler<GetAllAcademicYearsQuery, IReadOnlyList<AcademicYearDto>>
{
    public async Task<IReadOnlyList<AcademicYearDto>> Handle(GetAllAcademicYearsQuery query, CancellationToken cancellationToken)
    {
        var years = await unitOfWork.AcademicYears.ListAllAsync(cancellationToken);

        return mapper.Map<IReadOnlyList<AcademicYearDto>>(years);
    }
}
