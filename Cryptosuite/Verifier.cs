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
        private readonly ECDsaCurveType _curve;
        public string? Id { get; set; }
        public DerObjectIdentifier Algorithm { get; private set; }
        public AsymmetricKeyParameter PublicKey { get; set; }

        public Verifier(string? id, AsymmetricKeyParameter key, ECDsaCurveType curve)
        {
            Id = id;
            PublicKey = key;
            Algorithm = ECDsaCurve.ToDerObjectIdentifier(curve);
            _curve = curve;
            var digest = ECDsaCurve.GetDigest(curve);
            _verifier = new ECDsaSigner(new HMacDsaKCalculator(digest));
            _verifier.Init(false, PublicKey);
        }

        public virtual bool Verify(byte[] verifyData, byte[] signature)
        {
            var digest = ECDsaCurve.GetDigest(_curve);
            digest.BlockUpdate(verifyData, 0, verifyData.Length);
            var dataHash = new byte[digest.GetDigestSize()];
            digest.DoFinal(dataHash, 0);
            var halfLength = signature.Length / 2;
            var r = new BigInteger(1, signature[..halfLength]);
            var s = new BigInteger(1, signature[halfLength..]);
            return _verifier.VerifySignature(dataHash, r, s);
        }
    }
}
