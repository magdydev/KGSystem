using AutoMapper;
using KGSystem.Application.Enrollments.Dtos;
using KGSystem.Domain.Entities;

namespace KGSystem.Application.Enrollments;

public sealed class EnrollmentMappingProfile : Profile
{
    public EnrollmentMappingProfile()
    {
        CreateMap<Enrollment, EnrollmentDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.ChildNameAr, opt => opt.MapFrom(src => src.Child.FirstNameAr + " " + src.Child.LastNameAr))
            .ForMember(dest => dest.ChildNameEn, opt => opt.MapFrom(src => src.Child.FirstNameEn + " " + src.Child.LastNameEn))
            .ForMember(dest => dest.KGPhaseNameAr, opt => opt.MapFrom(src => src.KGPhase.NameAr))
            .ForMember(dest => dest.KGPhaseNameEn, opt => opt.MapFrom(src => src.KGPhase.NameEn))
            .ForMember(dest => dest.AcademicYearNameAr, opt => opt.MapFrom(src => src.AcademicYear.NameAr))
            .ForMember(dest => dest.AcademicYearNameEn, opt => opt.MapFrom(src => src.AcademicYear.NameEn));
    }
}
