using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthApi.Models;
using AuthApi.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AuthApi.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var auth = endpoints.MapGroup("/api/auth");

        auth.MapPost("/login", async (
            LoginRequest request,
            UserManager<ApplicationUser> userManager,
            IUserClaimsPrincipalFactory<ApplicationUser> principalFactory,
            IConfiguration configuration) =>
        {
            var user = await userManager.FindByNameAsync(request.UserName);
            if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
            {
                return Results.Unauthorized();
            }

            var principal = await principalFactory.CreateAsync(user);
            var expiresAt = DateTime.UtcNow.AddMinutes(30);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SigningKey"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = principal.Claims
                .Append(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiresAt,
                signingCredentials: credentials);

            return Results.Ok(new LoginResponse(
                new JwtSecurityTokenHandler().WriteToken(token),
                "Bearer",
                expiresAt));
        });

        auth.MapGet("/me", (ClaimsPrincipal user) => Results.Ok(new CurrentUserResponse(
                user.Identity?.Name ?? string.Empty,
                user.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToArray(),
                user.FindAll(Permissions.ClaimType).Select(claim => claim.Value).ToArray())))
            .RequireAuthorization();

        endpoints.MapGet("/api/reports", () => Results.Ok(new
            {
                report = "Monthly activity",
                totalUsers = 2,
                generatedAtUtc = DateTime.UtcNow
            }))
            .RequireAuthorization(AuthorizationPolicies.ReportsRead);

        endpoints.MapGet("/api/admin/users", async (UserManager<ApplicationUser> userManager) =>
            {
                var users = await userManager.Users
                    .OrderBy(user => user.UserName)
                    .Select(user => new DemoUserResponse(user.UserName!, user.Email!))
                    .ToListAsync();

                return Results.Ok(users);
            })
            .RequireAuthorization(AuthorizationPolicies.UsersManage);

        return endpoints;
    }
}

public sealed record LoginRequest(string UserName, string Password);

public sealed record LoginResponse(string AccessToken, string TokenType, DateTime ExpiresAtUtc);

public sealed record CurrentUserResponse(string UserName, string[] Roles, string[] Permissions);

public sealed record DemoUserResponse(string UserName, string Email);
