using KGSystem.Application.Common.Interfaces;
using KGSystem.Application.Payments.Dtos;

namespace KGSystem.Application.Payments.Queries.GetChildPayments;

public sealed record GetChildPaymentsQuery(Guid ChildId) : IQuery<IReadOnlyList<PaymentDto>>;
