using AutoMapper;
using KGSystem.Application.Attendance.Dtos;
using KGSystem.Domain.Entities;

namespace KGSystem.Application.Attendance;

public sealed class AttendanceMappingProfile : Profile
{
    public AttendanceMappingProfile()
    {
        CreateMap<Domain.Entities.Attendance, AttendanceDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.ChildNameAr, opt => opt.MapFrom(src => src.Child.FirstNameAr + " " + src.Child.LastNameAr))
            .ForMember(dest => dest.ChildNameEn, opt => opt.MapFrom(src => src.Child.FirstNameEn + " " + src.Child.LastNameEn));
    }
}
