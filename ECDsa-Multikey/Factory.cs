using Cryptosuite.Core;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ECDsa_Multikey
{
    internal class Factory
    {
        public static Signer CreateSigner(string id, AsymmetricCipherKeyPair key, string algorithm)
        {
            if (key is null)
            {
                throw new Exception("Secret key is required for signing.");
            }
            return new Signer(id, key, algorithm);
        }

        public static Verifier CreateVerifier(string id, AsymmetricCipherKeyPair key, string algorithm)
        {
            if (key is null)
            {
                throw new Exception("Public key is required for verification.");
            }
            return new Verifier(id, key, algorithm);
        }
    }
}
