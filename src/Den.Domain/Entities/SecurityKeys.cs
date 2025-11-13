namespace Den.Domain.Entities;

public class SecurityKey
{
    public Guid Id { get; set; }
    public required string KeyId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsRevoked { get; set; } = false;
    public DateTime? RevokedAt { get; set; }

    public required string EncryptedPrivateKey { get; set; }

    public enum SecurityKeyUsage { Sign, Encrypt }
    public required SecurityKeyUsage Usage { get; set; }

    public enum SecurityKeyType { RSA, ECDSA }
    public required SecurityKeyType KeyType { get; set; }

    public enum SecurityKeyHashAlgorithm { SHA256, SHA384, SHA512 }
    public SecurityKeyHashAlgorithm? HashAlgorithm { get; set; }

    public string? PublicJwkJson { get; set; }
}