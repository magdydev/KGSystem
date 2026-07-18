using AutoMapper;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Application.Payments.Dtos;
using KGSystem.Domain.Repositories;

namespace KGSystem.Application.Payments.Queries.GetPayments;

public sealed class GetPaymentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IQueryHandler<GetPaymentsQuery, IReadOnlyList<PaymentDto>>
{
    public async Task<IReadOnlyList<PaymentDto>> Handle(GetPaymentsQuery query, CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.Payment> payments;

        if (query.FromDate.HasValue && query.ToDate.HasValue)
        {
            payments = await unitOfWork.Payments.GetByDateRangeAsync(query.FromDate.Value, query.ToDate.Value, cancellationToken);
        }
        else
        {
            payments = await unitOfWork.Payments.ListAllAsync(cancellationToken);
        }

        var dtos = mapper.Map<List<PaymentDto>>(payments);

        if (query.ChildId.HasValue)
            dtos = dtos.Where(d => d.ChildNameAr.Contains(query.ChildId.ToString()!) || d.ChildNameEn.Contains(query.ChildId.ToString()!)).ToList();

        if (query.Month.HasValue)
            dtos = dtos.Where(d => d.Month == query.Month.Value).ToList();

        if (query.Year.HasValue)
            dtos = dtos.Where(d => d.Year == query.Year.Value).ToList();

        if (!string.IsNullOrWhiteSpace(query.Status))
            dtos = dtos.Where(d => d.Status == query.Status).ToList();

        return dtos.AsReadOnly();
    }
}
