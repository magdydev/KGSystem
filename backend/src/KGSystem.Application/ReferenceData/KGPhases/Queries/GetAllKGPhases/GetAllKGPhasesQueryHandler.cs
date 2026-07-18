using AutoMapper;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Application.ReferenceData.KGPhases.Dtos;
using KGSystem.Domain.Repositories;

namespace KGSystem.Application.ReferenceData.KGPhases.Queries.GetAllKGPhases;

public sealed class GetAllKGPhasesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IQueryHandler<GetAllKGPhasesQuery, IReadOnlyList<KGPhaseDto>>
{
    public async Task<IReadOnlyList<KGPhaseDto>> Handle(GetAllKGPhasesQuery query, CancellationToken cancellationToken)
    {
        var phases = await unitOfWork.KGPhases.GetOrderedAsync(cancellationToken);

        return mapper.Map<IReadOnlyList<KGPhaseDto>>(phases);
    }
}
