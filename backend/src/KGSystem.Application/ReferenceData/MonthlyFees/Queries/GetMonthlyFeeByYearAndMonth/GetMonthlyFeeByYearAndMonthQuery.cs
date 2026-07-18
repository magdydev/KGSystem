using KGSystem.Application.Common.Interfaces;
using KGSystem.Application.ReferenceData.MonthlyFees.Dtos;

namespace KGSystem.Application.ReferenceData.MonthlyFees.Queries.GetMonthlyFeeByYearAndMonth;

public sealed record GetMonthlyFeeByYearAndMonthQuery(Guid YearId, int Month) : IQuery<MonthlyFeeDto?>;
