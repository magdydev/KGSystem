using KGSystem.Application.Common.Interfaces;
using KGSystem.Application.Payments.Dtos;

namespace KGSystem.Application.Payments.Queries.GetPayments;

public sealed record GetPaymentsQuery(
    int? ChildId,
    int? Month,
    int? Year,
    string? Status,
    DateTime? FromDate,
    DateTime? ToDate) : IQuery<IReadOnlyList<PaymentDto>>;
