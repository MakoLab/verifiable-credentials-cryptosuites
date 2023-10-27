using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ECDsa_Multikey
{
    internal class Helpers
    {
        internal static string GetNamedCurveFromPublicMultikey(byte[] publicMultikey)
        {
            if (publicMultikey[0] == Constants.MulticodecP256PublicKeyHeader[0] &&
                publicMultikey[1] == Constants.MulticodecP256PublicKeyHeader[1])
            {
                return ECDsaCurve.P256;
            }
            if (publicMultikey[0] == Constants.MulticodecP384PublicKeyHeader[0] &&
                publicMultikey[1] == Constants.MulticodecP384PublicKeyHeader[1])
            {
                return ECDsaCurve.P384;
            }
            if (publicMultikey[0] == Constants.MulticodecP521PublicKeyHeader[0] &&
                publicMultikey[1] == Constants.MulticodecP521PublicKeyHeader[1])
            {
                return ECDsaCurve.P521;
            }
            throw new Exception("Unsupported public multikey header.");
        }

        internal static string GetNamedCurveFromSecretMultikey(byte[] secretMultikey)
        {
            if (secretMultikey[0] == Constants.MulticodecP256SecretKeyHeader[0] &&
                secretMultikey[1] == Constants.MulticodecP256SecretKeyHeader[1])
            {
                return ECDsaCurve.P256;
            }
            if (secretMultikey[0] == Constants.MulticodecP384SecretKeyHeader[0] &&
                secretMultikey[1] == Constants.MulticodecP384SecretKeyHeader[1])
            {
                return ECDsaCurve.P384;
            }
            if (secretMultikey[0] == Constants.MulticodecP521SecretKeyHeader[0] &&
                secretMultikey[1] == Constants.MulticodecP521SecretKeyHeader[1])
            {
                return ECDsaCurve.P521;
            }
            throw new Exception("Unsupported secret multikey header.");
        }

        internal static int GetSecretKeySize(ECDsa keys)
        {
            return keys.ExportParameters(true).Curve.Oid.FriendlyName switch
            {
                "nistP256" => 32,
                "nistP384" => 48,
                "nistP521" => 66,
                _ => throw new Exception("Unsupported curve.")
            };
        }

        internal static void SetSecretKeyHeader(ECDsa keys, byte[] buffer)
        {
            switch (keys.ExportParameters(true).Curve.Oid.FriendlyName)
            {
                case "nistP256":
                    Constants.MulticodecP256SecretKeyHeader.CopyTo(buffer.AsSpan());
                    break;
                case "nistP384":
                    Constants.MulticodecP384SecretKeyHeader.CopyTo(buffer.AsSpan());
                    break;
                case "nistP521":
                    Constants.MulticodecP521SecretKeyHeader.CopyTo(buffer.AsSpan());
                    break;
                default:
                    throw new Exception("Unsupported curve.");
            }
        }

        internal static void SetPublicKeyHeader(ECDsa keys, byte[] buffer)
        {
            switch (keys.ExportParameters(false).Curve.Oid.FriendlyName)
            {
                case "nistP256":
                    Constants.MulticodecP256PublicKeyHeader.CopyTo(buffer.AsSpan());
                    break;
                case "nistP384":
                    Constants.MulticodecP384PublicKeyHeader.CopyTo(buffer.AsSpan());
                    break;
                case "nistP521":
                    Constants.MulticodecP521PublicKeyHeader.CopyTo(buffer.AsSpan());
                    break;
                default:
                    throw new Exception("Unsupported curve.");
            }
        }
    }
}
