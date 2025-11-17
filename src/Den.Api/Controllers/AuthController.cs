using System.Security.Claims;
using Den.Application.Auth;
using Den.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Den.Api.Controllers;

[ApiController]
[Route("v1/auth")]
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

    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefreshRequest request)
    {
        var response = await authService.RefreshAsync(request);
        return response switch
        {
            null => Unauthorized(new { error = "invalid refresh token" }),
            _ => Ok(response),
        };
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> GetMe()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            logger.LogDebug("Invalid token: {Reason}", new { Reason = "Token has no subject ID" });
            return Unauthorized(new { error = "invalid token" });
        }

        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));

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