using Asp.Versioning;
using KGSystem.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KGSystem.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request.Username, request.Password);
        if (!result.Succeeded)
            return BadRequest(new { error = result.ErrorMessage });

        return Ok(new { token = result.Token, email = result.Email, roles = result.Roles });
    }

    [Authorize(Roles = "Manager")]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.RegisterAsync(request.Username, request.Password, request.Role);
        if (!result.Succeeded)
            return BadRequest(new { error = result.ErrorMessage });

        return Ok(new { token = result.Token, email = result.Email, roles = result.Roles });
    }
}

public sealed record LoginRequest(string Username, string Password);
public sealed record RegisterRequest(string Username, string Password, string Role);
