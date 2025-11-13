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

        if (builder.Environment.IsDevelopment()) options.RequireHttpsMetadata = false;
        options.Authority = "http://auth-api/.well-known/jwks.json";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
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
    var keys = await securityService.GetActiveKeysAsync(null, create: true);
    var jwks = keys.Select(k => securityService.GetPublicJsonWebKey(k));
    return Results.Json(new { Keys = jwks });
});

app.Run();