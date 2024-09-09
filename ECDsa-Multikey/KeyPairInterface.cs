using Cryptosuite.Core;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;

namespace ECDsa_Multikey
{
    public class KeyPairInterface
    {
        public required KeyPair KeyPair { get; set; }
        public required Verifier Verifier { get; set; }
        public Signer? Signer { get; set; }
    }
}