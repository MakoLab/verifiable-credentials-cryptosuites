using Cryptosuite.Core;
using Org.BouncyCastle.Crypto;

namespace ECDsa_Multikey
{
    internal class Factory
    {
        public static Signer CreateSigner(string id, AsymmetricCipherKeyPair key, ECDsaCurveType curve)
        {
            if (key is null)
            {
                throw new Exception("Secret key is required for signing.");
            }
            return new Signer(id, key.Private, curve);
        }

        public static Verifier CreateVerifier(string id, AsymmetricCipherKeyPair key, ECDsaCurveType curve)
        {
            if (key is null)
            {
                throw new Exception("Public key is required for verification.");
            }
            return new Verifier(id, key.Public, curve);
        }
    }
}
