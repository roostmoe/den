namespace Den.Application.Auth;

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresIn,
    string TokenType = "Bearer"
);