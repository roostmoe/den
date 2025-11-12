using System.Text;
using Den.Application.Auth;
using System.IdentityModel.Tokens.Jwt;
using Den.Auth.Api.Settings;
using Den.Domain.Entities;
using Den.Infrastructure.Auth;
using Den.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<AuthContext>(connectionName: "postgresdb");

builder.Services.AddScoped<IAuthService, AuthService>();

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

app.MapGet("/.well-known/jwks.json", () =>
{
    var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
        ?? throw new InvalidOperationException("Jwt not configured");

    var jwks = jwtOptions.Keys.Select(k => 
    {
        var jwk = k.GetJwk();
        var publicJwk = new JsonWebKey
        {
            Kty = jwk.Kty,
            Kid = jwk.Kid,
            Use = jwk.Use,
            Alg = jwk.Alg,
            
            // EC params
            Crv = jwk.Crv,
            X = jwk.X,
            Y = jwk.Y,
            
            // RSA params
            N = jwk.N,
            E = jwk.E
        };
        return publicJwk;
    });

    return Results.Json(new { Keys = jwks });
});

app.Run();