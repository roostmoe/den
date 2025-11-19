namespace Den.Domain.Entities;

public class Session
{
    public required Guid Id { get; set; }
    public required string RefreshTokenHash { get; set; }
    public DateTime Expiry { get; set; }

    public string? DeviceInfo { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}