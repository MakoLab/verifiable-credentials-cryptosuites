using Cryptosuite.Core;
using Org.BouncyCastle.Crypto;
using System.Security.Cryptography;

namespace ECDsa_Multikey
{
    public class KeyPairInterface
    {
        public string? Id { get; set; }
        public string? Controller { get; set; }
        public AsymmetricCipherKeyPair? Keys { get; set; }
        public string? Algorithm { get; set; }
        public string? PublicKeyMultibase { get; set; }
        public string? SecretKeyMultibase { get; set; }

        public Verifier? Verifier { get; set; }
        public Signer? Signer { get; set; }

        public MultikeyModel Export(bool publicKey = true, bool secretKey = false, bool includeContext = true)
        {
            var keyPair = new KeyPair
            {
                Id = Id,
                Controller = Controller,
                Keys = Keys,
                Algorithm = Algorithm,
            };
            return Serialize.ExportKeyPair(keyPair, publicKey, secretKey, includeContext);
        }
    }
}