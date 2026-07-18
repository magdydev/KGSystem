using FluentValidation;

namespace KGSystem.Application.Enrollments.Commands.CreateEnrollment;

public sealed class CreateEnrollmentCommandValidator : AbstractValidator<CreateEnrollmentCommand>
{
    public CreateEnrollmentCommandValidator()
    {
        RuleFor(v => v.ChildId)
            .NotEmpty().WithMessage("Child ID is required.");

        RuleFor(v => v.KGPhaseId)
            .NotEmpty().WithMessage("KG phase ID is required.");

        RuleFor(v => v.AcademicYearId)
            .NotEmpty().WithMessage("Academic year ID is required.");
    }
}
