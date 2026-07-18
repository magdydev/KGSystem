using Asp.Versioning;
using KGSystem.Application.Attendance.Commands.BatchRecordAttendance;
using KGSystem.Application.Attendance.Commands.RecordAttendance;
using KGSystem.Application.Attendance.Commands.UpdateAttendance;
using KGSystem.Application.Attendance.Dtos;
using KGSystem.Application.Attendance.Queries.GetAttendance;
using KGSystem.Application.Attendance.Queries.GetTodayAttendance;
using KGSystem.Application.Common;
using KGSystem.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KGSystem.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public sealed class AttendanceController(IDispatcher dispatcher) : ControllerBase
{
    [Authorize(Roles = "Accountant,Manager")]
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AttendanceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAttendance(
        [FromQuery] DateTime? date,
        [FromQuery] Guid? childId,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var query = new GetAttendanceQuery(date, childId, status);
        var result = await dispatcher.Send<IReadOnlyList<AttendanceDto>>(query, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Accountant,Manager")]
    [HttpGet("today")]
    [ProducesResponseType(typeof(IReadOnlyList<AttendanceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTodayAttendance(CancellationToken cancellationToken)
    {
        var result = await dispatcher.Send<IReadOnlyList<AttendanceDto>>(new GetTodayAttendanceQuery(), cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Accountant")]
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RecordAttendance([FromBody] RecordAttendanceCommand command, CancellationToken cancellationToken)
    {
        var attendanceId = await dispatcher.Send<Guid>(command, cancellationToken);
        return CreatedAtAction(nameof(GetTodayAttendance), null, attendanceId);
    }

    [Authorize(Roles = "Accountant")]
    [HttpPost("batch")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BatchRecordAttendance([FromBody] BatchRecordAttendanceCommand command, CancellationToken cancellationToken)
    {
        await dispatcher.Send<Unit>(command, cancellationToken);
        return Ok(new { success = true });
    }

    [Authorize(Roles = "Accountant")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAttendance(Guid id, [FromBody] UpdateAttendanceCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest(new { error = "Route ID does not match command ID." });

        await dispatcher.Send<Unit>(command, cancellationToken);
        return NoContent();
    }
}
