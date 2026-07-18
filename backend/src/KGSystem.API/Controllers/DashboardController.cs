using Asp.Versioning;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Application.Dashboard.Dtos;
using KGSystem.Application.Dashboard.Queries.GetDashboardSummary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KGSystem.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public sealed class DashboardController(IDispatcher dispatcher) : ControllerBase
{
    [Authorize(Roles = "Manager")]
    [HttpGet("summary")]
    [ProducesResponseType(typeof(DashboardSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var result = await dispatcher.Send<DashboardSummaryDto>(new GetDashboardSummaryQuery(), cancellationToken);
        return Ok(result);
    }
}
