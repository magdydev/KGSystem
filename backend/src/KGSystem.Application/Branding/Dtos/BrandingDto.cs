namespace KGSystem.Application.Branding.Dtos;

public sealed record BrandingDto
{
    public int Id { get; init; }
    public string AppName { get; init; } = string.Empty;
    public string AppNameAr { get; init; } = string.Empty;
    public string? LogoUrl { get; init; }
    public string? LogoData { get; init; }
    public string PrimaryColor { get; init; } = string.Empty;
    public string SecondaryColor { get; init; } = string.Empty;
    public string Currency { get; init; } = string.Empty;
}
