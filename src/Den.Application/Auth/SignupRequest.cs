namespace Den.Application.Auth;

public record SignupRequest(
  string Username,
  string DisplayName,
  string Email,
  string Password
);