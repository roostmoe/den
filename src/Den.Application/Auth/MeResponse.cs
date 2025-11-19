namespace Den.Application.Auth;

public record MeResponse(
    string Id,
    string Username,
    string DisplayName,
    string Email,
    string Role
);