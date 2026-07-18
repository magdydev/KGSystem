using AutoMapper;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Application.ReferenceData.MonthlyFees.Dtos;
using KGSystem.Domain.Repositories;

namespace KGSystem.Application.ReferenceData.MonthlyFees.Queries.GetMonthlyFeeByYearAndMonth;

public sealed class GetMonthlyFeeByYearAndMonthQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IQueryHandler<GetMonthlyFeeByYearAndMonthQuery, MonthlyFeeDto?>
{
    public async Task<MonthlyFeeDto?> Handle(GetMonthlyFeeByYearAndMonthQuery query, CancellationToken cancellationToken)
    {
        var fee = await unitOfWork.MonthlyFees.GetByYearAndMonthAsync(query.YearId, query.Month, cancellationToken);
        return fee is null ? null : mapper.Map<MonthlyFeeDto>(fee);
    }
}
