namespace Den.Application.Auth;

public interface IAuthService
{
  Task<AuthResponse> SignupAsync(SignupRequest request);
  Task<AuthResponse?> LoginAsync(LoginRequest request);
  Task<RefreshResponse?> RefreshAsync(RefreshRequest request);
}