using Asp.Versioning;
using KGSystem.Application.Common.Interfaces;
using KGSystem.Application.ReferenceData.AcademicYears.Commands.CreateAcademicYear;
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
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateAcademicYearCommand command, CancellationToken cancellationToken)
    {
        var yearId = await dispatcher.Send<Guid>(command, cancellationToken);
        return CreatedAtAction(nameof(GetAll), null, yearId);
    }
}
