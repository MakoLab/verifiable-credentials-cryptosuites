using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Cryptosuite.Core
{
    public class Verifier
    {
        private readonly ISigner _verifier;
        public string? Id { get; set; }
        public DerObjectIdentifier Algorithm { get; private set; }
        public AsymmetricCipherKeyPair Key { get; set; }

        public Verifier(string? id, AsymmetricCipherKeyPair key, string algorithm)
        {
            Id = id;
            Key = key;
            Algorithm = ECDsaCurve.ToDerObjectIdentifier(algorithm);
            _verifier = SignerUtilities.GetSigner(Algorithm);
            _verifier.Init(false, Key.Public);
        }

        public virtual bool Verify(byte[] verifyData, byte[] signature)
        {
            _verifier.BlockUpdate(verifyData, 0, verifyData.Length);
            var result = _verifier.VerifySignature(signature);
            _verifier.Reset();
            return result;
        }
    }
}
