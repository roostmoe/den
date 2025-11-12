using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace Den.Auth.Api.Settings;

public class JwtSettings
{
    public required List<JwtKey> Keys { get; set; }
}

public record JwtKey (
    JwtKeyAlg Alg,
    string PrivateKey
) {
    public JsonWebKey GetJwk()
    {
        // Decode the keys from Base64 to PEM
        var keyPemBytes = Convert.FromBase64String(PrivateKey);
        var keyPem = Encoding.UTF8.GetString(keyPemBytes);

        // Create a mutable algo container
        AsymmetricAlgorithm alg = Alg switch
        {
            JwtKeyAlg.RSA => RSA.Create(),
            JwtKeyAlg.ECDSA => ECDsa.Create(),
            _ => throw new NotImplementedException()
        };

        // Whatever the algo, import it from PEM
        alg.ImportFromPem(keyPem);

        // Generate a SecurityKey from the imported PEM
        SecurityKey key = Alg switch
        {
            JwtKeyAlg.RSA => new RsaSecurityKey((RSA) alg),
            JwtKeyAlg.ECDSA => new ECDsaSecurityKey((ECDsa) alg),
            _ => throw new NotImplementedException()
        };

        // Convert the SecurityKey into a JWK
        return JsonWebKeyConverter.ConvertFromSecurityKey(key);
    }

    public JsonWebKey GetPublicJwk()
    {
        var jwk = GetJwk();
        
        return new JsonWebKey
        {
            Kty = jwk.Kty,
            Use = "sig",
            Alg = DeriveAlgorithm(jwk),
            Kid = Guid.NewGuid().ToString(),
            
            // EC params
            Crv = jwk.Crv,
            X = jwk.X,
            Y = jwk.Y,
            
            // RSA params
            N = jwk.N,
            E = jwk.E
        };
    }

    private static string DeriveAlgorithm(JsonWebKey jwk)
    {
        return jwk.Kty switch
        {
            "EC" => jwk.Crv switch
            {
                "P-256" => SecurityAlgorithms.EcdsaSha256,
                "P-384" => SecurityAlgorithms.EcdsaSha384,
                "P-521" => SecurityAlgorithms.EcdsaSha512,
                _ => throw new NotSupportedException($"unsupported curve: {jwk.Crv}")
            },
            "RSA" => SecurityAlgorithms.RsaSha256, // could inspect key size if you're picky
            _ => throw new NotSupportedException($"unsupported key type: {jwk.Kty}")
        };
    }
};

public enum JwtKeyAlg {
    RSA = 1,
    ECDSA = 2
}