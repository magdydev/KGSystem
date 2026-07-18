using Asp.Versioning;
using KGSystem.Application.Common;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Application.Enrollments.Commands.CreateEnrollment;
using KGSystem.Application.Enrollments.Commands.UpdateEnrollment;
using KGSystem.Application.Enrollments.Dtos;
using KGSystem.Application.Enrollments.Queries.GetChildEnrollments;
using KGSystem.Application.Enrollments.Queries.GetEnrollments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KGSystem.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public sealed class EnrollmentsController(IDispatcher dispatcher) : ControllerBase
{
    [Authorize(Roles = "Accountant,Manager")]
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<EnrollmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEnrollments(CancellationToken cancellationToken)
    {
        var result = await dispatcher.Send<IReadOnlyList<EnrollmentDto>>(new GetEnrollmentsQuery(), cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Accountant,Manager")]
    [HttpGet("child/{childId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<EnrollmentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetChildEnrollments(Guid childId, CancellationToken cancellationToken)
    {
        var result = await dispatcher.Send<IReadOnlyList<EnrollmentDto>>(new GetChildEnrollmentsQuery(childId), cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Accountant")]
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateEnrollment([FromBody] CreateEnrollmentCommand command, CancellationToken cancellationToken)
    {
        var enrollmentId = await dispatcher.Send<Guid>(command, cancellationToken);
        return CreatedAtAction(nameof(GetChildEnrollments), new { childId = command.ChildId }, enrollmentId);
    }

    [Authorize(Roles = "Accountant")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateEnrollment(Guid id, [FromBody] UpdateEnrollmentCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest(new { error = "Route ID does not match command ID." });

        await dispatcher.Send<Unit>(command, cancellationToken);
        return NoContent();
    }
}
