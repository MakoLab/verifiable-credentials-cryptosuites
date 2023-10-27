using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDsa_Multikey
{
    internal static class Constants
    {
        internal const string Algorithm = "ECDSA";
        internal const bool IsExtractable = true;

        internal const string ECDsa2019Secp256KeyType = "EcdsaSecp256r1VerificationKey2019";
        internal const string ECDsa2019Secp384KeyType = "EcdsaSecp384r1VerificationKey2019";
        internal const string ECDsa2019Secp521KeyType = "EcdsaSecp521r1VerificationKey2019";

        internal const string ECDsa2019SuiteContextV1Url = "https://w3id.org/security/suites/ecdsa-2019/v1";
        internal const string MultikeyContextV1Url = "https://w3id.org/security/multikey/v1";

        internal const char MultibaseBase58Header = 'z';

        internal static readonly byte[] MulticodecP256PublicKeyHeader = new byte[] { 0x80, 0x24 };
        internal static readonly byte[] MulticodecP384PublicKeyHeader = new byte[] { 0x81, 0x24 };
        internal static readonly byte[] MulticodecP521PublicKeyHeader = new byte[] { 0x82, 0x24 };
        internal static readonly byte[] MulticodecP256SecretKeyHeader = new byte[] { 0x86, 0x26 };
        internal static readonly byte[] MulticodecP384SecretKeyHeader = new byte[] { 0x87, 0x26 };
        internal static readonly byte[] MulticodecP521SecretKeyHeader = new byte[] { 0x88, 0x26 };
    }

    internal static class ECDsaCurve
    {
        internal const string P256 = "P-256";
        internal const string P384 = "P-384";
        internal const string P521 = "P-521";
    }

    internal static class ECDsaHashFunction
    {
        internal const string SHA256 = "SHA-256";
        internal const string SHA384 = "SHA-384";
        internal const string SHA512 = "SHA-512";
    }
}
