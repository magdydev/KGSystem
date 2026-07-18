using FluentValidation;

namespace KGSystem.Application.ReferenceData.AcademicYears.Commands.CreateAcademicYear;

public sealed class CreateAcademicYearCommandValidator : AbstractValidator<CreateAcademicYearCommand>
{
    public CreateAcademicYearCommandValidator()
    {
        RuleFor(v => v.Code)
            .NotEmpty().WithMessage("Academic year code is required.");

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
