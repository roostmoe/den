using Den.Domain.Entities;

using Microsoft.IdentityModel.Tokens;

using MSSecurityKey = Microsoft.IdentityModel.Tokens.SecurityKey;
using SecurityKey = Den.Domain.Entities.SecurityKey;

namespace Den.Application.Auth;

public interface ISecurityService
{
    Task<SecurityKey?> GetKeyAsync(SecurityKey.SecurityKeyUsage usage, Guid? kid, bool create = true);
    Task<SecurityKey?> GetSigningKeyAsync(Guid? kid, bool create = true);
    Task<SecurityKey?> GetEncryptionKeyAsync(Guid? kid, bool create = true);

    public MSSecurityKey GetSecurityKey(SecurityKey key);

    public JsonWebKey GetJsonWebKey(SecurityKey key);
    public JsonWebKey GetPublicJsonWebKey(SecurityKey dbKey);
    public JsonWebKey GetPublicJsonWebKey(JsonWebKey key);
}