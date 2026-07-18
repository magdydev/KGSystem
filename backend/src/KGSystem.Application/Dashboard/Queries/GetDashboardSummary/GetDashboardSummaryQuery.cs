using KGSystem.Application.Common.Interfaces;
using KGSystem.Application.Dashboard.Dtos;

namespace KGSystem.Application.Dashboard.Queries.GetDashboardSummary;

public sealed record GetDashboardSummaryQuery : IQuery<DashboardSummaryDto>;
