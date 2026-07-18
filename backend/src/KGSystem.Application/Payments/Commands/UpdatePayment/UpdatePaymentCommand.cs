using KGSystem.Application.Common;
using KGSystem.Application.Common.Interfaces;

namespace KGSystem.Application.Payments.Commands.UpdatePayment;

public sealed record UpdatePaymentCommand(
    int Id,
    decimal AmountPaid,
    decimal Discount,
    string Method,
    string? Notes,
    string? ReceivedBy) : ICommand<Unit>;
