using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;
using System.Text;

namespace Cryptosuite.Core
{
    public class Signer
    {
        private readonly ISigner _signer;
        public string? Id { get; set; }
        public DerObjectIdentifier Algorithm { get; private set; }
        public AsymmetricCipherKeyPair Key { get; set; }

        public Signer(string? id, AsymmetricCipherKeyPair key, string algorithm)
        {
            Id = id;
            Key = key;
            Algorithm = ECDsaCurve.ToDerObjectIdentifier(algorithm);
            _signer = SignerUtilities.GetSigner(Algorithm);
            _signer.Init(true, Key.Private);
        }

        public byte[] Sign(byte[] data)
        {
            _signer.BlockUpdate(data, 0, data.Length);
            var signature = _signer.GenerateSignature();
            _signer.Reset();
            return signature;
        }
    }
}