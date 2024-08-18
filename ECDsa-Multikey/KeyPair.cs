using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;


namespace ECDsa_Multikey
{
    internal class KeyPair
    {
        public string? Id { get; set; }
        public string? Controller { get; set; }
        public AsymmetricCipherKeyPair? Keys { get; set; }
        public X9ECParameters? Curve { get; set; }
        public required string Algorithm { get; set; }
    }
}
