using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;


namespace ECDsa_Multikey
{
    internal class KeyPair
    {
        public required string Id { get; set; }
        public required string Controller { get; set; }
        public AsymmetricCipherKeyPair? Keys { get; set; }
        public X9ECParameters? Curve { get; set; }
        public string? Algorithm { get; set; }
    }
}
