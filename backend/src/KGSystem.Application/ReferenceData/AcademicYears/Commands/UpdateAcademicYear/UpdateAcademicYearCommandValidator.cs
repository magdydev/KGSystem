using FluentValidation;

namespace KGSystem.Application.ReferenceData.AcademicYears.Commands.UpdateAcademicYear;

public sealed class UpdateAcademicYearCommandValidator : AbstractValidator<UpdateAcademicYearCommand>
{
    public UpdateAcademicYearCommandValidator()
    {
        RuleFor(v => v.NameAr)
            .NotEmpty().WithMessage("Arabic academic year name is required.");

        RuleFor(v => v.NameEn)
            .NotEmpty().WithMessage("English academic year name is required.");

        RuleFor(v => v.StartDate)
            .NotEmpty().WithMessage("Start date is required.");

        RuleFor(v => v.EndDate)
            .NotEmpty().WithMessage("End date is required.")
            .GreaterThan(v => v.StartDate).WithMessage("End date must be after start date.");
    }
}
