using KGSystem.Application.Common.Interfaces;
using KGSystem.Application.ReferenceData.MonthlyFees.Dtos;

namespace KGSystem.Application.ReferenceData.MonthlyFees.Queries.GetAllMonthlyFees;

public sealed record GetAllMonthlyFeesQuery : IQuery<IReadOnlyList<MonthlyFeeDto>>;
