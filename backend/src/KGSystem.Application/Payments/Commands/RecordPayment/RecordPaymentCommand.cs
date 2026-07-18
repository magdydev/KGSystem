using KGSystem.Application.Common.Interfaces;

namespace KGSystem.Application.Payments.Commands.RecordPayment;

public sealed record RecordPaymentCommand(
    int EnrollmentId,
    int Month,
    int Year,
    decimal AmountDue,
    decimal AmountPaid,
    DateTime DueDate,
    string Method,
    string? Notes,
    string? ReceivedBy,
    DateTime? PaidDate,
    decimal Discount = 0) : ICommand<int>;
