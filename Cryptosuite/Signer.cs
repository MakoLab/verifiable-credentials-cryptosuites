using System.Security.Cryptography;
using System.Text;

namespace Cryptosuite.Core
{
    public class Signer
    {
        private readonly HashAlgorithmName _hashAlgorithmName;
        public string? Id { get; set; }
        public string Algorithm { get; private set; }
        public ECDsa Key { get; set; }

        public Signer(string? id, ECDsa key)
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

        public byte[] Sign(byte[] data)
        {
            return Key.SignData(data, _hashAlgorithmName);
        }
    }
}