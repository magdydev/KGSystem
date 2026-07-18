using AutoMapper;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Application.ReferenceData.MonthlyFees.Dtos;
using KGSystem.Domain.Repositories;

namespace KGSystem.Application.ReferenceData.MonthlyFees.Queries.GetAllMonthlyFees;

public sealed class GetAllMonthlyFeesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IQueryHandler<GetAllMonthlyFeesQuery, IReadOnlyList<MonthlyFeeDto>>
{
    public async Task<IReadOnlyList<MonthlyFeeDto>> Handle(GetAllMonthlyFeesQuery query, CancellationToken cancellationToken)
    {
        var fees = await unitOfWork.MonthlyFees.ListAllAsync(cancellationToken);

        return mapper.Map<IReadOnlyList<MonthlyFeeDto>>(fees);
    }
}
