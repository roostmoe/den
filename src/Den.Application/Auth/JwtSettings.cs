namespace Den.Application.Auth;

public class JwtSettings
{
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required string Secret { get; set; }
}
