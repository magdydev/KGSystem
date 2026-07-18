using Asp.Versioning;
using KGSystem.Application.Branding.Commands.UpdateBranding;
using KGSystem.Application.Branding.Dtos;
using KGSystem.Application.Branding.Queries.GetBranding;
using KGSystem.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KGSystem.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/settings")]
[Produces("application/json")]
public sealed class SettingsController(IDispatcher dispatcher) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("branding")]
    [ProducesResponseType(typeof(BrandingDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBranding(CancellationToken cancellationToken)
    {
        var result = await dispatcher.Send<BrandingDto?>(new GetBrandingQuery(), cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Manager")]
    [HttpPut("branding")]
    [ProducesResponseType(typeof(BrandingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateBranding([FromBody] UpdateBrandingCommand command, CancellationToken cancellationToken)
    {
        var result = await dispatcher.Send<BrandingDto>(command, cancellationToken);
        return Ok(result);
    }
}
