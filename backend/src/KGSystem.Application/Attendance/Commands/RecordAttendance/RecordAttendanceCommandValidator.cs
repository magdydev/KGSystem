using FluentValidation;

namespace KGSystem.Application.Attendance.Commands.RecordAttendance;

public sealed class RecordAttendanceCommandValidator : AbstractValidator<RecordAttendanceCommand>
{
    public RecordAttendanceCommandValidator()
    {
        RuleFor(v => v.ChildId)
            .NotEmpty().WithMessage("Child ID is required.");

        RuleFor(v => v.Date)
            .NotEmpty().WithMessage("Date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Attendance date cannot be in the future.");

        RuleFor(v => v.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(s => s is "Present" or "Absent" or "Excused" or "Late")
            .WithMessage("Status must be 'Present', 'Absent', 'Excused', or 'Late'.");
    }
}
