using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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
            Id = Guid.NewGuid(),
            Username = request.Username,
            DisplayName = request.DisplayName,
            Email = request.Email,
            PasswordHash = passwordHash,
            Role = UserRole.VIEWER
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var (accessToken, refreshToken, session) = await GenerateTokenPair(user);
        return new AuthResponse(accessToken, refreshToken, session.Expiry);
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        if (user is null || !VerifyPassword(request.Password, user.PasswordHash))
        {
            return null;
        }

        var (accessToken, refreshToken, session) = await GenerateTokenPair(user);
        return new AuthResponse(accessToken, refreshToken, session.Expiry);
    }

    public async Task<RefreshResponse?> RefreshAsync(RefreshRequest request)
    {
        var session = await context.Sessions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.RefreshTokenHash == HashRefreshToken(request.RefreshToken));
        if (session is null)
        {
            return null;
        }

        if (session.Expiry <= DateTime.UtcNow)
        {
            return null;
        }

        var accessToken = GenerateAccessToken(session.User);
        return new RefreshResponse(accessToken);
    }

    private static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    private async Task<(string AccessToken, string RefreshToken, Session Session)> GenerateTokenPair(User user)
    {
        var tokenExpiry = DateTime.UtcNow.AddDays(7);
        var refreshToken = GenerateRefreshToken();

        var session = new Session {
            Id = Guid.NewGuid(),
            RefreshTokenHash = HashRefreshToken(refreshToken),
            Expiry = tokenExpiry,
            UserId = user.Id,
        };
        context.Sessions.Add(session);
        await context.SaveChangesAsync();

        return (GenerateAccessToken(user), refreshToken, session);
    }

    private string GenerateAccessToken(User user)
    {
        var jwtSecret = config["Jwt:Secret"] ?? throw new InvalidOperationException("jwt secret not configured");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var now = DateTime.UtcNow;

        var accessToken = new JwtSecurityToken(
            issuer: config["jwt:Issuer"] ?? "den",
            audience: config["jwt:Audience"] ?? "den",
            claims: [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            ],
            expires: now.AddMinutes(5),
            signingCredentials: creds,
            notBefore: now
        );

        return new JwtSecurityTokenHandler().WriteToken(accessToken);
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public static string HashRefreshToken(string token)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashBytes);
    }

    public static bool VerifyRefreshToken(string a, string b)
    {
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(a),
            Encoding.UTF8.GetBytes(b)
        );
    }
}