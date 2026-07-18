using Asp.Versioning;
using KGSystem.Application.Common;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Application.ReferenceData.KGPhases.Commands.CreateKGPhase;
using KGSystem.Application.ReferenceData.KGPhases.Commands.UpdateKGPhase;
using KGSystem.Application.ReferenceData.KGPhases.Dtos;
using KGSystem.Application.ReferenceData.KGPhases.Queries.GetAllKGPhases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KGSystem.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public sealed class KGPhasesController(IDispatcher dispatcher) : ControllerBase
{
    [Authorize(Roles = "Accountant,Manager")]
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<KGPhaseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await dispatcher.Send<IReadOnlyList<KGPhaseDto>>(new GetAllKGPhasesQuery(), cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Manager")]
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateKGPhaseCommand command, CancellationToken cancellationToken)
    {
        var phaseId = await dispatcher.Send<int>(command, cancellationToken);
        return CreatedAtAction(nameof(GetAll), null, phaseId);
    }

    [Authorize(Roles = "Manager")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateKGPhaseCommand command, CancellationToken cancellationToken)
    {
        await dispatcher.Send<Unit>(command with { Id = id }, cancellationToken);
        return NoContent();
    }
}
