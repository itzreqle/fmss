using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Roza.AuthService.Services
{
    public interface IPublicRsaKeyProvider
    {
        RsaSecurityKey GetKey();
    }

    public interface IPrivateRsaKeyProvider
    {
        RsaSecurityKey GetKey();
    }
    public class PublicRsaKeyProvider : IPublicRsaKeyProvider
    {
        private readonly RsaSecurityKey _key;

        public PublicRsaKeyProvider()
        {
            var publicKey = File.ReadAllText("Resources/public.key");
            var rsa = RSA.Create();
            rsa.ImportFromPem(publicKey.ToCharArray());
            _key = new RsaSecurityKey(rsa);
        }

        public RsaSecurityKey GetKey() => _key;
    }

    public class PrivateRsaKeyProvider : IPrivateRsaKeyProvider
    {
        private readonly RsaSecurityKey _key;

        public PrivateRsaKeyProvider()
        {
            var privateKey = File.ReadAllText("Resources/private.key");
            var rsa = RSA.Create();
            rsa.ImportFromPem(privateKey.ToCharArray());
            _key = new RsaSecurityKey(rsa);
        }

        public RsaSecurityKey GetKey() => _key;
    }

}
