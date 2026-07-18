using AutoMapper;
using KGSystem.Application.Payments.Dtos;
using KGSystem.Domain.Entities;

namespace KGSystem.Application.Payments;

public sealed class PaymentMappingProfile : Profile
{
    public PaymentMappingProfile()
    {
        CreateMap<Payment, PaymentDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Method, opt => opt.MapFrom(src => src.Method.ToString()))
            .ForMember(dest => dest.ChildNameAr, opt => opt.MapFrom(src => src.Enrollment.Child.FirstNameAr + " " + src.Enrollment.Child.LastNameAr))
            .ForMember(dest => dest.ChildNameEn, opt => opt.MapFrom(src => src.Enrollment.Child.FirstNameEn + " " + src.Enrollment.Child.LastNameEn))
            .ForMember(dest => dest.KGPhaseNameAr, opt => opt.MapFrom(src => src.Enrollment.KGPhase.NameAr))
            .ForMember(dest => dest.KGPhaseNameEn, opt => opt.MapFrom(src => src.Enrollment.KGPhase.NameEn));
    }
}
