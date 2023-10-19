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

        internal static int GetSecretKeySize(CngKey keyPair)
        {
            
        }
    }
}
