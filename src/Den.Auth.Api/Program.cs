using System.Text;
using Den.Application.Auth;
using Den.Auth.Api.Settings;
using Den.Infrastructure.Auth;
using Den.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<AuthContext>(connectionName: "postgresdb");

builder.Services.AddDataProtection().SetApplicationName("den-auth");
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISecurityService, SecurityService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSecret = builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("jwt secret not configured");
        var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "den";
        var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "den";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultEndpoints();

app.MapGet("/.well-known/jwks.json", async (ISecurityService securityService) =>
{
    var keys = new List<Den.Domain.Entities.SecurityKey?> {
        await securityService.GetEncryptionKeyAsync(null),
        await securityService.GetEncryptionKeyAsync(null)
    };

    var jwks = keys.Select(k => k is null ? null : securityService.GetPublicJsonWebKey(k));
    return Results.Json(new { Keys = jwks });
});

app.Run();