namespace Den.Application.Auth;

public record RefreshResponse(
  string AccessToken,
  string TokenType = "Bearer"
);