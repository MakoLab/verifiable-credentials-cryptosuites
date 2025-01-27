﻿using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using System.Text;

namespace DI_Sd_Primitives
{
    public class HashingService
    {
        private readonly IDigest _digest;

        public HashingService()
        {
            _digest = new Sha256Digest();
        }

        public HashingService(IDigest digest)
        {
            _digest = digest;
        }

        public byte[] HashString(string input)
        {
            _digest.Reset();
             var data = Encoding.UTF8.GetBytes(input);
            _digest.BlockUpdate(data, 0, data.Length);
            var output = new byte[_digest.GetDigestSize()];
            _digest.DoFinal(output, 0);
            return output;
        }

        public byte[] HashMandatoryNQuads(IList<string> nQuads)
        {
            _digest.Reset();
            var input = Encoding.UTF8.GetBytes(String.Join("", nQuads));
            _digest.BlockUpdate(input, 0, input.Length);
            var output = new byte[_digest.GetDigestSize()];
            _digest.DoFinal(output, 0);
            return output;
        }

        public (HMac hMac, byte[] key) CreateHMAC()
        {
            var hmac = new HMac(_digest);
            var rnd = new DigestRandomGenerator(_digest);
            var key = new byte[_digest.GetDigestSize()];
            rnd.NextBytes(key);
            hmac.Init(new KeyParameter(key));
            return (hmac, key);
        }

        public HMac CreateHMAC(byte[] key)
        {
            var hmac = new HMac(_digest);
            hmac.Init(new KeyParameter(key));
            return hmac;
        }
    }
}
