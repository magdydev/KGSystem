using FluentValidation;

namespace KGSystem.Application.ReferenceData.KGPhases.Commands.CreateKGPhase;

public sealed class CreateKGPhaseCommandValidator : AbstractValidator<CreateKGPhaseCommand>
{
    public CreateKGPhaseCommandValidator()
    {
        RuleFor(v => v.Code)
            .NotEmpty().WithMessage("Phase code is required.");

        RuleFor(v => v.NameAr)
            .NotEmpty().WithMessage("Arabic phase name is required.");

        RuleFor(v => v.NameEn)
            .NotEmpty().WithMessage("English phase name is required.");

        RuleFor(v => v.SortOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Sort order cannot be negative.");
    }
}
