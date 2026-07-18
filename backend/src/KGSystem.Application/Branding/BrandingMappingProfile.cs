using AutoMapper;
using KGSystem.Application.Branding.Dtos;
using KGSystem.Domain.Entities;

namespace KGSystem.Application.Branding;

public sealed class BrandingMappingProfile : Profile
{
    public BrandingMappingProfile()
    {
        CreateMap<BrandingSetting, BrandingDto>();
    }
}
