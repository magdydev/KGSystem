using AutoMapper;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Application.Payments.Dtos;
using KGSystem.Domain.Repositories;

namespace KGSystem.Application.Payments.Queries.GetChildPayments;

public sealed class GetChildPaymentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IQueryHandler<GetChildPaymentsQuery, IReadOnlyList<PaymentDto>>
{
    public async Task<IReadOnlyList<PaymentDto>> Handle(GetChildPaymentsQuery query, CancellationToken cancellationToken)
    {
        var enrollments = await unitOfWork.Enrollments.GetByChildAsync(query.ChildId, cancellationToken);
        var enrollmentIds = enrollments.Select(e => e.Id).ToHashSet();

        var allPayments = await unitOfWork.Payments.ListAllAsync(cancellationToken);
        var filtered = allPayments.Where(p => enrollmentIds.Contains(p.EnrollmentId)).ToList();

        return mapper.Map<IReadOnlyList<PaymentDto>>(filtered);
    }
}
