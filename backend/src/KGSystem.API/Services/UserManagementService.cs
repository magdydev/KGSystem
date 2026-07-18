using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KGSystem.API.Services;

public interface IUserManagementService
{
    Task<IReadOnlyList<UserSummary>> GetAllUsersAsync();
    Task<IReadOnlyList<string>> GetAllRolesAsync();
    Task<UserManagementResult> CreateUserAsync(string username, string password, IReadOnlyList<string> roles);
    Task<UserManagementResult> UpdateUserRolesAsync(string userId, IReadOnlyList<string> roles);
    Task<UserManagementResult> DeleteUserAsync(string userId, string currentUserId);
}

public sealed record UserSummary(string Id, string UserName, string Email, IList<string> Roles);

public sealed record UserManagementResult(bool Succeeded, string? ErrorMessage);

public sealed class UserManagementService(
    UserManager<IdentityUser> userManager,
    RoleManager<IdentityRole> roleManager) : IUserManagementService
{
    public async Task<IReadOnlyList<UserSummary>> GetAllUsersAsync()
    {
        var users = await userManager.Users.OrderBy(u => u.UserName).ToListAsync();
        var summaries = new List<UserSummary>();

        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            summaries.Add(new UserSummary(user.Id, user.UserName!, user.Email!, roles));
        }

        return summaries;
    }

    public async Task<IReadOnlyList<string>> GetAllRolesAsync()
    {
        return await roleManager.Roles.Select(r => r.Name!).OrderBy(name => name).ToListAsync();
    }

    public async Task<UserManagementResult> CreateUserAsync(string username, string password, IReadOnlyList<string> roles)
    {
        if (string.IsNullOrWhiteSpace(username))
            return new UserManagementResult(false, "Username is required.");

        if (await userManager.FindByNameAsync(username) is not null)
            return new UserManagementResult(false, "Username already exists.");

        var invalidRole = await FindInvalidRoleAsync(roles);
        if (invalidRole is not null)
            return new UserManagementResult(false, $"Role '{invalidRole}' does not exist.");

        var user = new IdentityUser { UserName = username, Email = $"{username}@kgsystem.com", EmailConfirmed = true };
        var createResult = await userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
            return new UserManagementResult(false, string.Join(" ", createResult.Errors.Select(e => e.Description)));

        if (roles.Count > 0)
            await userManager.AddToRolesAsync(user, roles);

        return new UserManagementResult(true, null);
    }

    public async Task<UserManagementResult> UpdateUserRolesAsync(string userId, IReadOnlyList<string> roles)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return new UserManagementResult(false, "User not found.");

        var invalidRole = await FindInvalidRoleAsync(roles);
        if (invalidRole is not null)
            return new UserManagementResult(false, $"Role '{invalidRole}' does not exist.");

        if (!roles.Contains("Manager") && await IsLastManagerAsync(user))
            return new UserManagementResult(false, "Cannot remove the Manager role from the last remaining manager.");

        var currentRoles = await userManager.GetRolesAsync(user);
        var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
            return new UserManagementResult(false, string.Join(" ", removeResult.Errors.Select(e => e.Description)));

        if (roles.Count > 0)
        {
            var addResult = await userManager.AddToRolesAsync(user, roles);
            if (!addResult.Succeeded)
                return new UserManagementResult(false, string.Join(" ", addResult.Errors.Select(e => e.Description)));
        }

        return new UserManagementResult(true, null);
    }

    public async Task<UserManagementResult> DeleteUserAsync(string userId, string currentUserId)
    {
        if (string.Equals(userId, currentUserId, StringComparison.OrdinalIgnoreCase))
            return new UserManagementResult(false, "You cannot delete your own account.");

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return new UserManagementResult(false, "User not found.");

        if (await IsLastManagerAsync(user))
            return new UserManagementResult(false, "Cannot delete the last remaining manager.");

        var result = await userManager.DeleteAsync(user);
        return result.Succeeded
            ? new UserManagementResult(true, null)
            : new UserManagementResult(false, string.Join(" ", result.Errors.Select(e => e.Description)));
    }

    private async Task<string?> FindInvalidRoleAsync(IReadOnlyList<string> roles)
    {
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                return role;
        }

        return null;
    }

    private async Task<bool> IsLastManagerAsync(IdentityUser user)
    {
        var userRoles = await userManager.GetRolesAsync(user);
        if (!userRoles.Contains("Manager"))
            return false;

        var managers = await userManager.GetUsersInRoleAsync("Manager");
        return managers.Count <= 1;
    }
}
