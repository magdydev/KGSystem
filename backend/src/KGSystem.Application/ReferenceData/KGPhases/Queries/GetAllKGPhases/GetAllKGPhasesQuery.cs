using KGSystem.Application.Common.Interfaces;
using KGSystem.Application.ReferenceData.KGPhases.Dtos;

namespace KGSystem.Application.ReferenceData.KGPhases.Queries.GetAllKGPhases;

public sealed record GetAllKGPhasesQuery : IQuery<IReadOnlyList<KGPhaseDto>>;
