using System.Security.Cryptography;
using System.Text;

using Den.Application.Auth;
using Den.Infrastructure.Persistence;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using MSSecurityKey = Microsoft.IdentityModel.Tokens.SecurityKey;
using SecurityKey = Den.Domain.Entities.SecurityKey;

namespace Den.Infrastructure.Auth;

public class SecurityService(
    AuthContext dbContext,
    IDataProtectionProvider dataProtector
) : ISecurityService
{
    public async Task<List<SecurityKey>> GetActiveKeysAsync(SecurityKey.SecurityKeyUsage? usage, bool create = true)
    {
        var query = dbContext.SecurityKeys.AsQueryable();

        if (usage.HasValue)
            query = query.Where(sk => sk.Usage == usage);

        query = query.Where(sk => sk.IsActive && !sk.IsRevoked && (sk.ExpiresAt == null || sk.ExpiresAt > DateTime.UtcNow));

        var keys = await query.ToListAsync();

        if (
            !keys.Any(x => x.Usage == SecurityKey.SecurityKeyUsage.Sign)
            && usage != SecurityKey.SecurityKeyUsage.Encrypt
            && create
        )
        {
            // We don't have any active keys for this usage type, we need to make one.
            var signingKey = await GenerateKey(
                usage ?? SecurityKey.SecurityKeyUsage.Sign,
                SecurityKey.SecurityKeyType.ECDSA,
                usage == SecurityKey.SecurityKeyUsage.Sign ? SecurityKey.SecurityKeyHashAlgorithm.SHA512 : null
            );
            keys.Add(signingKey);
        }

        if (
            !keys.Any(x => x.Usage == SecurityKey.SecurityKeyUsage.Encrypt)
            && usage != SecurityKey.SecurityKeyUsage.Sign
            && create
        )
        {
            // We don't have any active keys for this usage type, we need to make one.
            var signingKey = await GenerateKey(
                usage ?? SecurityKey.SecurityKeyUsage.Encrypt,
                SecurityKey.SecurityKeyType.ECDSA,
                usage == SecurityKey.SecurityKeyUsage.Encrypt ? SecurityKey.SecurityKeyHashAlgorithm.SHA512 : null
            );
            keys.Add(signingKey);
        }

        return keys;
    }

    /// <summary>
    /// Retrieves a signing key by its Key ID (kid).
    /// If kid is null, retrieves the active encryption key.
    ///
    /// If create is true and no key is found, a new key will be generated.
    /// </summary>
    public async Task<SecurityKey?> GetKeyAsync(SecurityKey.SecurityKeyUsage usage, Guid? kid, bool create = true)
    {
        var query = dbContext.SecurityKeys
            .Where(sk => sk.Usage == usage);

        query = kid.HasValue
            ? query.Where(sk => sk.Id == kid.Value)
            : query.Where(sk => sk.IsActive && !sk.IsRevoked && (sk.ExpiresAt == null || sk.ExpiresAt > DateTime.UtcNow));

        var key = query.FirstOrDefault();

        if (key is null && create)
        {
            // We don't have an active key for this usage type, we need to make one.
            key = await GenerateKey(
                usage,
                SecurityKey.SecurityKeyType.ECDSA,
                usage == SecurityKey.SecurityKeyUsage.Sign ? SecurityKey.SecurityKeyHashAlgorithm.SHA512 : null
            );
        }

        return key;
    }

    /// <summary>
    /// Retrieves a signing key by its Key ID (kid).
    /// If kid is null, retrieves the active encryption key.
    /// </summary>
    public Task<SecurityKey?> GetSigningKeyAsync(Guid? kid, bool create = true) => GetKeyAsync(SecurityKey.SecurityKeyUsage.Sign, kid, create);

    /// <summary>
    /// Retrieves a encryption key by its Key ID (kid).
    /// If kid is null, retrieves the active encryption key.
    /// </summary>
    public Task<SecurityKey?> GetEncryptionKeyAsync(Guid? kid, bool create = true) => GetKeyAsync(SecurityKey.SecurityKeyUsage.Encrypt, kid, create);

    private async Task<SecurityKey> GenerateKey(SecurityKey.SecurityKeyUsage usage, SecurityKey.SecurityKeyType keyType, SecurityKey.SecurityKeyHashAlgorithm? hashAlgorithm = null, DateTime? expiresAt = null)
    {
        var kid = Guid.NewGuid().ToString();
        string pemKey = keyType switch {
            SecurityKey.SecurityKeyType.RSA => GenerateRsaPrivateKey(),
            SecurityKey.SecurityKeyType.ECDSA => GenerateEcdsaPrivateKey(),
            _ => throw new NotSupportedException($"Unsupported key type: {keyType}")
        };

        var privateKeyEncrypted = EncryptString(pemKey);

        var securityKey = new SecurityKey
        {
            Id = Guid.NewGuid(),
            KeyId = kid,
            KeyType = keyType,
            HashAlgorithm = hashAlgorithm,
            EncryptedPrivateKey = privateKeyEncrypted,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt,
            Usage = usage
        };

        // Persist the new key to the database.
        dbContext.SecurityKeys.Add(securityKey);
        await dbContext.SaveChangesAsync();

        return securityKey;
    }

    /// <summary>
    /// Generates a new RSA private key and returns it along with its PEM representation.
    /// </summary>
    private static string GenerateRsaPrivateKey()
    {
        using var rsa = RSA.Create(4096);
        var pem = PemEncoding.Write("RSA PRIVATE KEY", rsa.ExportRSAPrivateKey());
        var privateKeyPem = Encoding.UTF8.GetBytes(pem);
        var privateKeyBase64 = Convert.ToBase64String(privateKeyPem);
        return privateKeyBase64;
    }

    /// <summary>
    /// Generates a new ECDSA private key and returns it along with its PEM representation.
    /// </summary>
    private static string GenerateEcdsaPrivateKey()
    {
        using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP521);
        var pem = PemEncoding.Write("EC PRIVATE KEY", ecdsa.ExportECPrivateKey());
        var privateKeyPem = Encoding.UTF8.GetBytes(pem);
        var privateKeyBase64 = Convert.ToBase64String(privateKeyPem);
        return privateKeyBase64;
    }

    private AsymmetricAlgorithm GetAlgorithm(SecurityKey key)
    {
        AsymmetricAlgorithm algo = key.KeyType switch
        {
            SecurityKey.SecurityKeyType.RSA => RSA.Create(),
            SecurityKey.SecurityKeyType.ECDSA => ECDsa.Create(),
            _ => throw new NotSupportedException($"Unsupported key type: {key.KeyType}")
        };
        algo.ImportFromPem(Encoding.UTF8.GetString(Convert.FromBase64String(DecryptString(key.EncryptedPrivateKey))));
        return algo;
    }

    public MSSecurityKey GetSecurityKey(SecurityKey key)
    {
        return key.KeyType switch
        {
            SecurityKey.SecurityKeyType.RSA => new RsaSecurityKey((RSA)GetAlgorithm(key)),
            SecurityKey.SecurityKeyType.ECDSA => new ECDsaSecurityKey((ECDsa)GetAlgorithm(key)),
            _ => throw new NotSupportedException($"Unsupported key type: {key.KeyType}")
        };
    }

    public SigningCredentials GetSigningCredentials(SecurityKey key)
    {
        var secKey = GetSecurityKey(key);
        var signingCreds = new SigningCredentials(secKey, GetSecurityAlgorithms(key))
        {
            CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
        };
        return signingCreds;
    }

    public JsonWebKey GetJsonWebKey(SecurityKey key)
    {
        var securityKey = GetSecurityKey(key);
        JsonWebKey outKey = key.KeyType switch
        {
            SecurityKey.SecurityKeyType.RSA => JsonWebKeyConverter.ConvertFromRSASecurityKey((RsaSecurityKey)securityKey),
            SecurityKey.SecurityKeyType.ECDSA => JsonWebKeyConverter.ConvertFromECDsaSecurityKey((ECDsaSecurityKey)securityKey),
            _ => throw new NotSupportedException($"Unsupported key type: {key.KeyType}")
        };
        outKey.Kid = key.KeyId;
        outKey.Alg = GetSecurityAlgorithms(key);
        outKey.Use = key.Usage switch {
            SecurityKey.SecurityKeyUsage.Sign => "sig",
            SecurityKey.SecurityKeyUsage.Encrypt => "enc",
            _ => throw new NotSupportedException($"Unsupported key usage: {key.Usage}")
        };
        return outKey;
    }

    public JsonWebKey GetPublicJsonWebKey(SecurityKey dbKey)
    {
        var key = GetJsonWebKey(dbKey);
        return GetPublicJsonWebKey(key);
    }

    public JsonWebKey GetPublicJsonWebKey(JsonWebKey key)
    {
        // Copy existing key but exclude private parameters
        var outKey = new JsonWebKey
        {
            Kid = key.Kid,
            Kty = key.Kty,
            Use = key.Use,
            Alg = key.Alg,
            Crv = key.Crv,
            X = key.X,
            Y = key.Y,
            E = key.E,
            N = key.N
        };
        return outKey;
    }

    private static string GetSecurityAlgorithms(SecurityKey key)
    {
        return key.HashAlgorithm switch {
            SecurityKey.SecurityKeyHashAlgorithm.SHA256 => SecurityAlgorithms.RsaSha256,
            SecurityKey.SecurityKeyHashAlgorithm.SHA384 => SecurityAlgorithms.RsaSha384,
            SecurityKey.SecurityKeyHashAlgorithm.SHA512 => SecurityAlgorithms.RsaSha512,
            _ => key.KeyType switch {
                SecurityKey.SecurityKeyType.RSA => SecurityAlgorithms.RsaSha256,
                SecurityKey.SecurityKeyType.ECDSA => SecurityAlgorithms.EcdsaSha512,
                _ => throw new NotSupportedException($"Unsupported key type: {key.KeyType}")
            }
        };
    }

    /// <summary>
    /// Encrypts a string using the data protector.
    /// </summary>
    private string EncryptString(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var encBytes = dataProtector.CreateProtector("dek").Protect(bytes);
        return Convert.ToBase64String(encBytes);
    }

    /// <summary>
    /// Decrypts a string using the data protector.
    /// </summary>
    private string DecryptString(string encryptedInput)
    {
        var encBytes = Convert.FromBase64String(encryptedInput);
        var bytes = dataProtector.CreateProtector("dek").Unprotect(encBytes);
        return Encoding.UTF8.GetString(bytes);
    }
}