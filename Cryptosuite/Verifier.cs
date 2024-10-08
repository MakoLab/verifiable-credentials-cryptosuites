using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace Cryptosuite.Core
{
    public class Verifier
    {
        private readonly ISigner _verifier;
        public string? Id { get; set; }
        public DerObjectIdentifier Algorithm { get; private set; }
        public AsymmetricKeyParameter PublicKey { get; set; }

        public Verifier(string? id, AsymmetricKeyParameter key, ECDsaCurveType curve)
        {
            Id = id;
            PublicKey = key;
            Algorithm = ECDsaCurve.ToDerObjectIdentifier(curve);
            _verifier = SignerUtilities.GetSigner(curve.ToDerObjectIdentifier());
            _verifier.Init(false, PublicKey);
        }

        public virtual bool Verify(byte[] verifyData, byte[] signature)
        {
            _verifier.BlockUpdate(verifyData);
            return _verifier.VerifySignature(signature);
        }
    }
}
