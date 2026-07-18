using System.Security.Claims;
using Asp.Versioning;
using KGSystem.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KGSystem.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
[Authorize(Roles = "Manager")]
public sealed class UsersController(IUserManagementService userManagementService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UserSummary>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var users = await userManagementService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("roles")]
    [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await userManagementService.GetAllRolesAsync();
        return Ok(roles);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        var result = await userManagementService.CreateUserAsync(request.Username, request.Password, request.Roles);
        if (!result.Succeeded)
            return BadRequest(new { error = result.ErrorMessage });

        return NoContent();
    }

    [HttpPut("{id}/roles")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateRoles(string id, [FromBody] UpdateUserRolesRequest request)
    {
        var result = await userManagementService.UpdateUserRolesAsync(id, request.Roles);
        if (!result.Succeeded)
            return BadRequest(new { error = result.ErrorMessage });

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(string id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var result = await userManagementService.DeleteUserAsync(id, currentUserId);
        if (!result.Succeeded)
            return BadRequest(new { error = result.ErrorMessage });

        return NoContent();
    }
}

public sealed record CreateUserRequest(string Username, string Password, IReadOnlyList<string> Roles);
public sealed record UpdateUserRolesRequest(IReadOnlyList<string> Roles);
