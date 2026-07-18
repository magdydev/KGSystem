using FluentValidation;

namespace KGSystem.Application.Payments.Commands.RecordPayment;

public sealed class RecordPaymentCommandValidator : AbstractValidator<RecordPaymentCommand>
{
    public RecordPaymentCommandValidator()
    {
        RuleFor(v => v.EnrollmentId)
            .NotEmpty().WithMessage("Enrollment ID is required.");

        RuleFor(v => v.Month)
            .InclusiveBetween(1, 12).WithMessage("Month must be between 1 and 12.");

        RuleFor(v => v.Year)
            .GreaterThanOrEqualTo(2000).WithMessage("Invalid year.");

        RuleFor(v => v.AmountDue)
            .GreaterThanOrEqualTo(0).WithMessage("Amount due cannot be negative.");

        RuleFor(v => v.AmountPaid)
            .GreaterThanOrEqualTo(0).WithMessage("Amount paid cannot be negative.");

        RuleFor(v => v.DueDate)
            .NotEmpty().WithMessage("Due date is required.");

        RuleFor(v => v.Method)
            .NotEmpty().WithMessage("Payment method is required.")
            .Must(m => m is "Cash" or "Card" or "Transfer").WithMessage("Method must be 'Cash', 'Card', or 'Transfer'.");
    }
}
