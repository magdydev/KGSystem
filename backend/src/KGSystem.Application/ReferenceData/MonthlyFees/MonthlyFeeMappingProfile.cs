using AutoMapper;
using KGSystem.Application.ReferenceData.MonthlyFees.Dtos;
using KGSystem.Domain.Entities;

namespace KGSystem.Application.ReferenceData.MonthlyFees;

public sealed class MonthlyFeeMappingProfile : Profile
{
    public MonthlyFeeMappingProfile()
    {
        CreateMap<MonthlyFee, MonthlyFeeDto>()
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Amount))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Amount.Currency))
            .ForMember(dest => dest.AcademicYearNameAr, opt => opt.MapFrom(src => src.AcademicYear.NameAr))
            .ForMember(dest => dest.AcademicYearNameEn, opt => opt.MapFrom(src => src.AcademicYear.NameEn));
    }
}
