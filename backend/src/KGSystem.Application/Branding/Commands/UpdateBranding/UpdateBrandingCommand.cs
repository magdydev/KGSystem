using KGSystem.Application.Branding.Dtos;
using KGSystem.Application.Common.Interfaces;

namespace KGSystem.Application.Branding.Commands.UpdateBranding;

public sealed record UpdateBrandingCommand(
    string AppName,
    string AppNameAr,
    string PrimaryColor,
    string SecondaryColor,
    string Currency,
    string? LogoUrl = null,
    string? LogoData = null) : ICommand<BrandingDto>;
