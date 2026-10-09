using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Compare_TicketApi;

// Demo-Anmeldung mit einem Benutzer aus der Konfiguration; in echten Projekten: Benutzer-DB und Passwort-Hashing (ASP.NET Identity).
public class AuthService(IConfiguration config)
{
    public LoginResponse? Login(LoginRequest request)
    {
        var user = config["DemoUser:Username"] ?? "";
        var password = config["DemoUser:Password"] ?? "";

        // Konstante Vergleichszeit gegen Timing-Angriffe.
        var userOk = CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(request.Username), Encoding.UTF8.GetBytes(user));
        var passOk = CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(request.Password), Encoding.UTF8.GetBytes(password));
        if (!userOk || !passOk) return null;

        var expires = DateTime.UtcNow.AddMinutes(config.GetValue("Jwt:ExpiresMinutes", 60));
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: [new Claim(ClaimTypes.Name, user)],
            expires: expires,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new LoginResponse(new JwtSecurityTokenHandler().WriteToken(token), user, expires);
    }
}
