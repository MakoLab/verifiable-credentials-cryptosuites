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
        private readonly HashAlgorithmName _hashAlgorithmName;
        public string? Id { get; set; }
        public string Algorithm { get; private set; }
        public ECDsa Key { get; set; }

        public Verifier(string? id, ECDsa key)
        {
            _hashAlgorithmName = key.KeySize switch
            {
                256 => HashAlgorithmName.SHA256,
                384 => HashAlgorithmName.SHA384,
                512 => HashAlgorithmName.SHA512,
                _ => throw new System.Exception("Unsupported key size.")
            };
            Id = id;
            Key = key;
            Algorithm = ECDsaCurve.FromECCurve(key.ExportParameters(false).Curve);
        }

        public bool Verify(byte[] verifyData, byte[] signature)
        {
            return Key.VerifyData(verifyData, signature, _hashAlgorithmName);
        }
    }
}
