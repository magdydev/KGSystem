using FluentValidation;

namespace KGSystem.Application.Branding.Commands.UpdateBranding;

public sealed class UpdateBrandingCommandValidator : AbstractValidator<UpdateBrandingCommand>
{
    private const string HexColorPattern = "^#([0-9A-Fa-f]{6}|[0-9A-Fa-f]{3})$";

    public UpdateBrandingCommandValidator()
    {
        RuleFor(x => x.AppName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.AppNameAr)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.LogoUrl)
            .MaximumLength(2048);

        RuleFor(x => x.PrimaryColor)
            .NotEmpty()
            .Matches(HexColorPattern)
            .WithMessage("Primary color must be a valid hex color, e.g. #6366f1.");

        RuleFor(x => x.SecondaryColor)
            .NotEmpty()
            .Matches(HexColorPattern)
            .WithMessage("Secondary color must be a valid hex color, e.g. #f59e0b.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3)
            .WithMessage("Currency must be a 3-letter ISO 4217 code.");
    }
}
