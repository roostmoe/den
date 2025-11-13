using Microsoft.IdentityModel.Tokens;

using MSSecurityKey = Microsoft.IdentityModel.Tokens.SecurityKey;
using SecurityKey = Den.Domain.Entities.SecurityKey;

namespace Den.Application.Auth;

public interface ISecurityService
{
    Task<List<SecurityKey>> GetActiveKeysAsync(SecurityKey.SecurityKeyUsage? usage, bool create = true);
    Task<SecurityKey?> GetKeyAsync(SecurityKey.SecurityKeyUsage usage, Guid? kid, bool create = true);
    Task<SecurityKey?> GetSigningKeyAsync(Guid? kid, bool create = true);
    Task<SecurityKey?> GetEncryptionKeyAsync(Guid? kid, bool create = true);

    MSSecurityKey GetSecurityKey(SecurityKey key);
    SigningCredentials GetSigningCredentials(SecurityKey key);

    JsonWebKey GetJsonWebKey(SecurityKey key);
    JsonWebKey GetPublicJsonWebKey(SecurityKey dbKey);
    JsonWebKey GetPublicJsonWebKey(JsonWebKey key);
}