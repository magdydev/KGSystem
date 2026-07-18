using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KGSystem.API.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace KGSystem.API.Services;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(string email, string password);
    Task<AuthResult> RegisterAsync(string email, string password, string role);
}

public sealed record AuthResult(bool Succeeded, string Token, string Email, IList<string> Roles, string? ErrorMessage);

public sealed class AuthService(
    UserManager<IdentityUser> userManager,
    RoleManager<IdentityRole> roleManager,
    JwtSettings jwtSettings) : IAuthService
{
    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        var user = await userManager.FindByNameAsync(email) ?? await userManager.FindByEmailAsync(email);
        if (user == null || !await userManager.CheckPasswordAsync(user, password))
        {
            return new AuthResult(false, string.Empty, string.Empty, [], "AUTH.INVALID_CREDENTIALS");
        }

        var roles = await userManager.GetRolesAsync(user);
        var token = await GenerateJwtTokenAsync(user, roles);

        return new AuthResult(true, token, user.Email!, roles, null);
    }

    public async Task<AuthResult> RegisterAsync(string username, string password, string role)
    {
        if (await userManager.FindByNameAsync(username) != null)
        {
            return new AuthResult(false, string.Empty, string.Empty, [], "Username already exists.");
        }

        if (!await roleManager.RoleExistsAsync(role))
        {
            return new AuthResult(false, string.Empty, string.Empty, [], $"Role '{role}' does not exist.");
        }

        var user = new IdentityUser { UserName = username, Email = $"{username}@kgsystem.com", EmailConfirmed = true };
        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new AuthResult(false, string.Empty, string.Empty, [], errors);
        }

        await userManager.AddToRoleAsync(user, role);
        var roles = new List<string> { role };
        var token = await GenerateJwtTokenAsync(user, roles);

        return new AuthResult(true, token, user.Email!, roles, null);
    }

    private Task<string> GenerateJwtTokenAsync(IdentityUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            string.IsNullOrEmpty(jwtSettings.SigningKey) ? new string('0', 32) : jwtSettings.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtSettings.ExpiryMinutes),
            signingCredentials: credentials);

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }
}
