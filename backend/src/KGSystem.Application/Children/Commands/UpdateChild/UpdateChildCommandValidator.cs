using FluentValidation;

namespace KGSystem.Application.Children.Commands.UpdateChild;

public sealed class UpdateChildCommandValidator : AbstractValidator<UpdateChildCommand>
{
    public UpdateChildCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Child ID is required.");

        RuleFor(v => v.FirstNameAr)
            .NotEmpty().WithMessage("Arabic first name is required.");

        RuleFor(v => v.FirstNameEn)
            .NotEmpty().WithMessage("English first name is required.");

        RuleFor(v => v.LastNameAr)
            .NotEmpty().WithMessage("Arabic last name is required.");

        RuleFor(v => v.LastNameEn)
            .NotEmpty().WithMessage("English last name is required.");

        RuleFor(v => v.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.")
            .LessThan(DateTime.UtcNow).WithMessage("Date of birth cannot be in the future.");

        RuleFor(v => v.Gender)
            .NotEmpty().WithMessage("Gender is required.")
            .Must(g => g is "Male" or "Female").WithMessage("Gender must be 'Male' or 'Female'.");

        RuleFor(v => v.GuardianNameAr)
            .NotEmpty().WithMessage("Arabic guardian name is required.");

        RuleFor(v => v.GuardianNameEn)
            .NotEmpty().WithMessage("English guardian name is required.");

        RuleFor(v => v.GuardianPhone)
            .NotEmpty().WithMessage("Guardian phone is required.");
    }
}
