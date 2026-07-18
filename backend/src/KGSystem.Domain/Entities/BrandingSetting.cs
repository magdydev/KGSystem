using System.Text.RegularExpressions;
using KGSystem.Domain.Common;
using KGSystem.Domain.Events;
using KGSystem.Domain.Exceptions;

namespace KGSystem.Domain.Entities;

public sealed partial class BrandingSetting : BaseEntity, IAggregateRoot
{
    public string AppName { get; private set; } = null!;
    public string AppNameAr { get; private set; } = null!;
    public string? LogoUrl { get; private set; }
    public string? LogoData { get; private set; }
    public string PrimaryColor { get; private set; } = null!;
    public string SecondaryColor { get; private set; } = null!;
    public string Currency { get; private set; } = null!;

    private BrandingSetting()
    {
    }

    public static BrandingSetting CreateDefault(string appName, string appNameAr, string? logoUrl, string? logoData, string primaryColor, string secondaryColor, string currency)
    {
        var settings = new BrandingSetting();
        settings.Update(appName, appNameAr, logoUrl, logoData, primaryColor, secondaryColor, currency);
        return settings;
    }

    public void Update(string appName, string appNameAr, string? logoUrl, string? logoData, string primaryColor, string secondaryColor, string currency)
    {
        if (string.IsNullOrWhiteSpace(appName))
            throw new DomainException("Application name is required.");

        if (string.IsNullOrWhiteSpace(appNameAr))
            throw new DomainException("Arabic application name is required.");

        if (!IsValidHexColor(primaryColor))
            throw new DomainException($"'{primaryColor}' is not a valid hex color.");

        if (!IsValidHexColor(secondaryColor))
            throw new DomainException($"'{secondaryColor}' is not a valid hex color.");

        if (string.IsNullOrWhiteSpace(currency) || currency.Trim().Length != 3)
            throw new DomainException("Currency must be a 3-letter ISO 4217 code.");

        var trimmedLogoUrl = string.IsNullOrWhiteSpace(logoUrl) ? null : logoUrl.Trim();
        var trimmedLogoData = string.IsNullOrWhiteSpace(logoData) ? null : logoData.Trim();
        var changed = AppName != appName || AppNameAr != appNameAr || LogoUrl != trimmedLogoUrl || LogoData != trimmedLogoData || PrimaryColor != primaryColor || SecondaryColor != secondaryColor || Currency != currency;

        AppName = appName.Trim();
        AppNameAr = appNameAr.Trim();
        LogoUrl = trimmedLogoUrl;
        LogoData = trimmedLogoData;
        PrimaryColor = primaryColor.Trim();
        SecondaryColor = secondaryColor.Trim();
        Currency = currency.Trim().ToUpperInvariant();

        if (changed)
        {
            AddDomainEvent(new BrandingSettingsUpdatedDomainEvent(Id, AppName));
        }
    }

    private static bool IsValidHexColor(string? value) =>
        !string.IsNullOrWhiteSpace(value) && HexColorRegex().IsMatch(value);

    [GeneratedRegex("^#([0-9A-Fa-f]{6}|[0-9A-Fa-f]{3})$")]
    private static partial Regex HexColorRegex();
}
