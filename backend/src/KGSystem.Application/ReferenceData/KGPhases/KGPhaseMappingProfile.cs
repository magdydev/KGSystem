using AutoMapper;
using KGSystem.Application.ReferenceData.KGPhases.Dtos;
using KGSystem.Domain.Entities;

namespace KGSystem.Application.ReferenceData.KGPhases;

public sealed class KGPhaseMappingProfile : Profile
{
    public KGPhaseMappingProfile()
    {
        CreateMap<KGPhase, KGPhaseDto>();
    }
}
