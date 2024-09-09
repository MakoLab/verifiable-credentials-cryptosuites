using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;

namespace Cryptosuite.Core
{
    public class Signer
    {
        private readonly ECDsaSigner _signer;
        public string? Id { get; set; }
        public DerObjectIdentifier Algorithm { get; private set; }
        public AsymmetricKeyParameter PrivateKey { get; set; }

        public Signer(string? id, AsymmetricKeyParameter privateKey, string algorithm)
        {
            Id = id;
            PrivateKey = privateKey;
            Algorithm = ECDsaCurve.ToDerObjectIdentifier(algorithm);
            _signer = new ECDsaSigner(new HMacDsaKCalculator(new Sha256Digest()));
            _signer.Init(true, PrivateKey);
        }

        public byte[] Sign(byte[] data)
        {
            var digest = new Sha256Digest();
            digest.BlockUpdate(data, 0, data.Length);
            var dataHash = new byte[digest.GetDigestSize()];
            digest.DoFinal(dataHash, 0);
            var signature = _signer.GenerateSignature(dataHash);

            var sigArray = new byte[64];
            var r = signature[0].ToByteArrayUnsigned();
            var s = signature[1].ToByteArrayUnsigned();
            r.CopyTo(sigArray, 32 - r.Length);
            s.CopyTo(sigArray, 64 - s.Length);
            return sigArray;
        }
    }
}