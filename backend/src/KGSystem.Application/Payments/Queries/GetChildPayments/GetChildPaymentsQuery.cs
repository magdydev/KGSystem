using KGSystem.Application.Common.Interfaces;
using KGSystem.Application.Payments.Dtos;

namespace KGSystem.Application.Payments.Queries.GetChildPayments;

public sealed record GetChildPaymentsQuery(int ChildId) : IQuery<IReadOnlyList<PaymentDto>>;
