using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Den.Application.Auth;
using Den.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Den.Auth.Api.Controllers;

[ApiController]
[Route("v1")]
public class AuthController(
    IAuthService authService,
    AuthContext context,
    ILogger<AuthController> logger
) : ControllerBase
{
    [HttpPost("signup")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Signup([FromBody] SignupRequest request)
    {
        try
        {
            var response = await authService.SignupAsync(request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var response = await authService.LoginAsync(request);
        return response switch
        {
            null => Unauthorized(new { error = "invalid credentials" }),
            _ => Ok(response),
        };
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> GetMe()
    {
        var tokenType = User.FindFirstValue(JwtRegisteredClaimNames.Typ);
        if (tokenType is null || tokenType != "acc")
        {
            logger.LogDebug("Invalid token: {Reason}", new { Reason = "Token type is null or non-acc" });
            return Unauthorized(new { error = "invalid token" });
        }

        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (userId is null)
        {
            logger.LogDebug("Invalid token: {Reason}", new { Reason = "Token has no subject ID" });
            return Unauthorized(new { error = "invalid token" });
        }

        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == int.Parse(userId));

        if (user is null)
        {
            return Unauthorized(new { error = "user not found" });
        }

        return Ok(new
        {
            id = user.Id,
            username = user.Username,
            displayName = user.DisplayName,
            email = user.Email,
            role = user.Role.ToString()
        });
    }
}