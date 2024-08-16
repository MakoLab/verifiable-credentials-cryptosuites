using Cryptosuite.Core;
using Org.BouncyCastle.Crypto;
using SimpleBase;
using System.Security.Cryptography;

namespace ECDsa_Multikey
{
    public class KeyPairInterface
    {
        public string? Id { get; set; }
        public string? Controller { get; set; }
        public AsymmetricCipherKeyPair? Keys { get; set; }
        public required string Algorithm { get; set; }
        public required Verifier Verifier { get; set; }
        public Signer? Signer { get; set; }

        public MultikeyVerificationMethod Export(bool includePublicKey = true, bool includeSecretKey = false, bool includeContext = true)
        {
            var keyPair = new KeyPair
            {
                Id = Id,
                Controller = Controller,
                Keys = Keys,
                Algorithm = Algorithm,
            };
            return Serialize.ExportKeyPair(keyPair, includePublicKey, includeSecretKey, includeContext);
        }
    }
}