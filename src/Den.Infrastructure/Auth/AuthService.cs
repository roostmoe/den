using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Den.Application.Auth;
using Den.Domain.Entities;
using Den.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Den.Infrastructure.Auth;

public class AuthService(AuthContext context, IConfiguration config) : IAuthService
{
    public async Task<AuthResponse> SignupAsync(SignupRequest request)
    {
        if (await context.Users.AnyAsync(u => u.Email == request.Email))
        {
            throw new InvalidOperationException("email already in use");
        }

        if (await context.Users.AnyAsync(u => u.Username == request.Username))
        {
            throw new InvalidOperationException("username already taken");
        }

        var passwordHash = HashPassword(request.Password);

        var user = new User
        {
            Id = 0,
            Username = request.Username,
            DisplayName = request.DisplayName,
            Email = request.Email,
            PasswordHash = passwordHash,
            Role = UserRole.VIEWER
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var accessExpires = DateTime.UtcNow.AddHours(1);
        var accessToken = GenerateJwtToken(user, accessExpires, false);
        var refreshToken = GenerateJwtToken(user, DateTime.UtcNow.AddDays(7), true);

        return new AuthResponse(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            ExpiresIn: accessExpires
        );
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user is null || !VerifyPassword(request.Password, user.PasswordHash))
        {
            return null;
        }

        var accessExpires = DateTime.UtcNow.AddHours(3);
        var accessToken = GenerateJwtToken(user, accessExpires, false);
        var refreshToken = GenerateJwtToken(user, DateTime.UtcNow.AddDays(7), true);

        return new AuthResponse(accessToken, refreshToken, accessExpires);
    }

    private static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    private string GenerateJwtToken(User user, DateTime expiry, bool refresh)
    {
        var jwtSecret = config["Jwt:Secret"] ?? throw new InvalidOperationException("jwt secret not configured");
        var jwtIssuer = config["Jwt:Issuer"] ?? "den";
        var jwtAudience = config["Jwt:Audience"] ?? "den";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Typ, refresh ? "ref" : "acc")
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: expiry,
            signingCredentials: creds,
            notBefore: DateTime.UtcNow
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}