using Asp.Versioning;
using KGSystem.Application.Common;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Application.ReferenceData.AcademicYears.Commands.CreateAcademicYear;
using KGSystem.Application.ReferenceData.AcademicYears.Commands.UpdateAcademicYear;
using KGSystem.Application.ReferenceData.AcademicYears.Dtos;
using KGSystem.Application.ReferenceData.AcademicYears.Queries.GetAllAcademicYears;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KGSystem.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public sealed class AcademicYearsController(IDispatcher dispatcher) : ControllerBase
{
    [Authorize(Roles = "Accountant,Manager")]
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AcademicYearDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await dispatcher.Send<IReadOnlyList<AcademicYearDto>>(new GetAllAcademicYearsQuery(), cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Manager")]
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateAcademicYearCommand command, CancellationToken cancellationToken)
    {
        var yearId = await dispatcher.Send<int>(command, cancellationToken);
        return CreatedAtAction(nameof(GetAll), null, yearId);
    }

    [Authorize(Roles = "Manager")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAcademicYearCommand command, CancellationToken cancellationToken)
    {
        await dispatcher.Send<Unit>(command with { Id = id }, cancellationToken);
        return NoContent();
    }
}
