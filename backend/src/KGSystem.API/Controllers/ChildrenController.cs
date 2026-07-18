using Asp.Versioning;
using KGSystem.Application.Common;
using KGSystem.Application.Children.Commands.CreateChild;
using KGSystem.Application.Children.Commands.DeleteChild;
using KGSystem.Application.Children.Commands.UpdateChild;
using KGSystem.Application.Children.Dtos;
using KGSystem.Application.Children.Queries.GetChildById;
using KGSystem.Application.Children.Queries.GetChildren;
using KGSystem.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KGSystem.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public sealed class ChildrenController(IDispatcher dispatcher) : ControllerBase
{
    [Authorize(Roles = "Accountant,Manager")]
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ChildDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetChildren(
        [FromQuery] string? searchTerm,
        [FromQuery] string? status,
        [FromQuery] Guid? phaseId,
        CancellationToken cancellationToken)
    {
        var query = new GetChildrenQuery(searchTerm, status, phaseId);
        var result = await dispatcher.Send<IReadOnlyList<ChildDto>>(query, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Accountant,Manager")]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ChildDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetChildById(Guid id, CancellationToken cancellationToken)
    {
        var result = await dispatcher.Send<ChildDetailDto>(new GetChildByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Accountant")]
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateChild([FromBody] CreateChildCommand command, CancellationToken cancellationToken)
    {
        var childId = await dispatcher.Send<Guid>(command, cancellationToken);
        return CreatedAtAction(nameof(GetChildById), new { id = childId }, childId);
    }

    [Authorize(Roles = "Accountant")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateChild(Guid id, [FromBody] UpdateChildCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest(new { error = "Route ID does not match command ID." });

        await dispatcher.Send<Unit>(command, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Accountant")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteChild(Guid id, CancellationToken cancellationToken)
    {
        await dispatcher.Send<Unit>(new DeleteChildCommand(id), cancellationToken);
        return NoContent();
    }
}
