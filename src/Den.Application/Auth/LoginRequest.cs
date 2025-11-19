namespace Den.Application.Auth;

public record LoginRequest(
  string Username,
  string Password
);