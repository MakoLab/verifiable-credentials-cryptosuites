using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;

namespace Cryptosuite.Core
{
    public class Verifier
    {
        private readonly ECDsaSigner _verifier;
        public string? Id { get; set; }
        public DerObjectIdentifier Algorithm { get; private set; }
        public AsymmetricKeyParameter PublicKey { get; set; }

        public Verifier(string? id, AsymmetricKeyParameter key, string algorithm)
        {
            Id = id;
            PublicKey = key;
            Algorithm = ECDsaCurve.ToDerObjectIdentifier(algorithm);
            _verifier = new ECDsaSigner(new HMacDsaKCalculator(new Sha256Digest()));
            _verifier.Init(false, PublicKey);
        }

        public virtual bool Verify(byte[] verifyData, byte[] signature)
        {
            var digest = new Sha256Digest();
            digest.BlockUpdate(verifyData, 0, verifyData.Length);
            var dataHash = new byte[digest.GetDigestSize()];
            digest.DoFinal(dataHash, 0);
            var r = new BigInteger(1, signature[..32]);
            var s = new BigInteger(1, signature[32..]);
            return _verifier.VerifySignature(dataHash, r, s);
        }
    }
}
