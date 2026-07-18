using FluentValidation;

namespace KGSystem.Application.ReferenceData.MonthlyFees.Commands.PatchMonthlyFees;

public sealed class PatchMonthlyFeesCommandValidator : AbstractValidator<PatchMonthlyFeesCommand>
{
    public PatchMonthlyFeesCommandValidator()
    {
        RuleFor(v => v.AcademicYearId)
            .NotEmpty().WithMessage("Academic year ID is required.");

        RuleFor(v => v.Fees)
            .NotEmpty().WithMessage("At least one fee is required.");

        RuleForEach(v => v.Fees).ChildRules(fee =>
        {
            fee.RuleFor(x => x.Month)
                .InclusiveBetween(1, 12).WithMessage("Month must be between 1 and 12.");

            fee.RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(0).WithMessage("Amount cannot be negative.");
        });
    }
}
