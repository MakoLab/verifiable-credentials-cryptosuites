using Cryptosuite.Core;
using System.Security.Cryptography;

namespace ECDsa_Multikey
{
    public class KeyPairInterface
    {
        public string? Id { get; set; }
        public string? Controller { get; set; }
        public ECDsa? Keys { get; set; }
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
            };
            return Serialize.ExportKeyPair(keyPair, publicKey, secretKey, includeContext);
        }
    }
}