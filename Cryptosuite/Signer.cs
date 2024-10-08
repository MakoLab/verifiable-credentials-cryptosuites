using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;

namespace Cryptosuite.Core
{
    public class Signer
    {
        private readonly ISigner _signer;
        private readonly ECDsaCurveType _curve;
        public string? Id { get; set; }
        public DerObjectIdentifier Algorithm { get; private set; }
        public AsymmetricKeyParameter PrivateKey { get; set; }

        public Signer(string? id, AsymmetricKeyParameter privateKey, ECDsaCurveType curve)
        {
            Id = id;
            PrivateKey = privateKey;
            Algorithm = ECDsaCurve.ToDerObjectIdentifier(curve);
            _curve = curve;
            var digest = ECDsaCurve.GetDigest(curve);
            _signer = SignerUtilities.GetSigner(curve.ToDerObjectIdentifier());
            _signer.Init(true, PrivateKey);
        }

        public byte[] Sign(byte[] data)
        {
            var digest = ECDsaCurve.GetDigest(_curve);
            digest.BlockUpdate(data, 0, data.Length);
            var dataHash = new byte[digest.GetDigestSize()];
            digest.DoFinal(dataHash, 0);
            _signer.BlockUpdate(data);
            return _signer.GenerateSignature();
        }
    }
}