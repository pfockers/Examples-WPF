using System.Security.Claims;
using AuthApi.Models;
using AuthApi.Security;
using Microsoft.AspNetCore.Identity;

namespace AuthApi.Seeding;

public static class DemoDataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        await EnsureRoleAsync(roleManager, "Reader", [Permissions.ReportsRead]);
        await EnsureRoleAsync(roleManager, "Administrator", [Permissions.ReportsRead, Permissions.UsersManage]);

        await EnsureUserAsync(userManager, "reader@demo.local", "Reader123!", "Reader");
        await EnsureUserAsync(userManager, "admin@demo.local", "Admin123!", "Administrator");
    }

    private static async Task EnsureRoleAsync(
        RoleManager<IdentityRole> roleManager,
        string roleName,
        IEnumerable<string> permissions)
    {
        var role = await roleManager.FindByNameAsync(roleName);
        if (role is null)
        {
            role = new IdentityRole(roleName);
            var result = await roleManager.CreateAsync(role);
            EnsureSucceeded(result);
        }

        var existingClaims = await roleManager.GetClaimsAsync(role);
        foreach (var permission in permissions)
        {
            if (!existingClaims.Any(claim => claim.Type == Permissions.ClaimType && claim.Value == permission))
            {
                var result = await roleManager.AddClaimAsync(role, new Claim(Permissions.ClaimType, permission));
                EnsureSucceeded(result);
            }
        }
    }

    private static async Task EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        string userName,
        string password,
        string roleName)
    {
        var user = await userManager.FindByNameAsync(userName);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = userName,
                Email = userName,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);
            EnsureSucceeded(result);
        }

        if (!await userManager.IsInRoleAsync(user, roleName))
        {
            var result = await userManager.AddToRoleAsync(user, roleName);
            EnsureSucceeded(result);
        }
    }

    private static void EnsureSucceeded(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(error => error.Description)));
        }
    }
}
