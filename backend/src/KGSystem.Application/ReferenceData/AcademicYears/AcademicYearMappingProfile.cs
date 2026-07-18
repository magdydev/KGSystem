using AutoMapper;
using KGSystem.Application.ReferenceData.AcademicYears.Dtos;
using KGSystem.Domain.Entities;

namespace KGSystem.Application.ReferenceData.AcademicYears;

public sealed class AcademicYearMappingProfile : Profile
{
    public AcademicYearMappingProfile()
    {
        CreateMap<AcademicYear, AcademicYearDto>();
    }
}
