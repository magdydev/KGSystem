using FluentValidation;

namespace KGSystem.Application.Attendance.Commands.BatchRecordAttendance;

public sealed class BatchRecordAttendanceCommandValidator : AbstractValidator<BatchRecordAttendanceCommand>
{
    public BatchRecordAttendanceCommandValidator()
    {
        RuleFor(v => v.Date)
            .NotEmpty().WithMessage("Date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Attendance date cannot be in the future.");

        RuleFor(v => v.Records)
            .NotEmpty().WithMessage("At least one attendance record is required.");

        RuleForEach(v => v.Records).ChildRules(record =>
        {
            record.RuleFor(r => r.ChildId)
                .NotEmpty().WithMessage("Child ID is required.");

            record.RuleFor(r => r.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(s => s is "Present" or "Absent" or "Excused" or "Late")
                .WithMessage("Status must be 'Present', 'Absent', 'Excused', or 'Late'.");
        });
    }
}
