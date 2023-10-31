using Cryptosuite.Core;
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
        public static Signer CreateSigner(string id, ECDsa secretKey)
        {
            if (secretKey is null)
            {
                throw new Exception("Secret key is required for signing.");
            }
            return new Signer(id, secretKey);
        }

        public static Verifier CreateVerifier(string id, ECDsa publicKey)
        {
            if (publicKey is null)
            {
                throw new Exception("Public key is required for verification.");
            }
            return new Verifier(id, publicKey);
        }
    }
}
