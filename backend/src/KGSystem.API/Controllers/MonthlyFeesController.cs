using Asp.Versioning;
using KGSystem.Application.Common;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Application.ReferenceData.MonthlyFees.Commands.PatchMonthlyFees;
using KGSystem.Application.ReferenceData.MonthlyFees.Dtos;
using KGSystem.Application.ReferenceData.MonthlyFees.Queries.GetAllMonthlyFees;
using KGSystem.Application.ReferenceData.MonthlyFees.Queries.GetMonthlyFeeByYearAndMonth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KGSystem.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public sealed class MonthlyFeesController(IDispatcher dispatcher) : ControllerBase
{
    [Authorize(Roles = "Accountant,Manager")]
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<MonthlyFeeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await dispatcher.Send<IReadOnlyList<MonthlyFeeDto>>(new GetAllMonthlyFeesQuery(), cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Accountant,Manager")]
    [HttpGet("by-year/{yearId:guid}")]
    [ProducesResponseType(typeof(MonthlyFeeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByYearAndMonth(
        Guid yearId,
        [FromQuery] int month,
        CancellationToken cancellationToken)
    {
        var result = await dispatcher.Send<MonthlyFeeDto?>(new GetMonthlyFeeByYearAndMonthQuery(yearId, month), cancellationToken);
        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [Authorize(Roles = "Manager")]
    [HttpPatch("by-year/{yearId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PatchByYear(Guid yearId, [FromBody] PatchMonthlyFeesCommand command, CancellationToken cancellationToken)
    {
        if (yearId != command.AcademicYearId)
            return BadRequest(new { error = "Route year ID does not match command year ID." });

        await dispatcher.Send<Unit>(command, cancellationToken);
        return Ok(new { success = true });
    }
}
