﻿using Cryptosuite.Core;

namespace ECDsa_Multikey
{
    internal class Helpers
    {
        internal static ECDsaCurveType GetNamedCurveFromPublicMultikey(ReadOnlySpan<byte> publicMultikey)
        {
            if (publicMultikey[0] == Constants.MulticodecP256PublicKeyHeader[0] &&
                publicMultikey[1] == Constants.MulticodecP256PublicKeyHeader[1])
            {
                return ECDsaCurveType.P256;
            }
            if (publicMultikey[0] == Constants.MulticodecP384PublicKeyHeader[0] &&
                publicMultikey[1] == Constants.MulticodecP384PublicKeyHeader[1])
            {
                return ECDsaCurveType.P384;
            }
            if (publicMultikey[0] == Constants.MulticodecP521PublicKeyHeader[0] &&
                publicMultikey[1] == Constants.MulticodecP521PublicKeyHeader[1])
            {
                return ECDsaCurveType.P521;
            }
            throw new Exception("Unsupported public multikey header.");
        }

        internal static ECDsaCurveType GetNamedCurveFromSecretMultikey(ReadOnlySpan<byte> secretMultikey)
        {
            if (secretMultikey[0] == Constants.MulticodecP256SecretKeyHeader[0] &&
                secretMultikey[1] == Constants.MulticodecP256SecretKeyHeader[1])
            {
                return ECDsaCurveType.P256;
            }
            if (secretMultikey[0] == Constants.MulticodecP384SecretKeyHeader[0] &&
                secretMultikey[1] == Constants.MulticodecP384SecretKeyHeader[1])
            {
                return ECDsaCurveType.P384;
            }
            if (secretMultikey[0] == Constants.MulticodecP521SecretKeyHeader[0] &&
                secretMultikey[1] == Constants.MulticodecP521SecretKeyHeader[1])
            {
                return ECDsaCurveType.P521;
            }
            throw new Exception("Unsupported secret multikey header.");
        }

        internal static int GetSecretKeySize(string? algorithm)
        {
            return algorithm switch
            {
                ECDsaCurve.P256 => 32,
                ECDsaCurve.P384 => 48,
                ECDsaCurve.P521 => 66,
                _ => throw new Exception("Unsupported curve.")
            };
        }

        internal static int GetSecretKeySize(ECDsaCurveType curve)
        {
            return curve switch
            {
                ECDsaCurveType.P256 => 32,
                ECDsaCurveType.P384 => 48,
                ECDsaCurveType.P521 => 66,
                _ => throw new Exception("Unsupported curve.")
            };
        }

        internal static void SetSecretKeyHeader(string? algorithm, Span<byte> buffer)
        {
            switch (algorithm)
            {
                case ECDsaCurve.P256:
                    Constants.MulticodecP256SecretKeyHeader.CopyTo(buffer);
                    break;
                case ECDsaCurve.P384:
                    Constants.MulticodecP384SecretKeyHeader.CopyTo(buffer);
                    break;
                case ECDsaCurve.P521:
                    Constants.MulticodecP521SecretKeyHeader.CopyTo(buffer);
                    break;
                default:
                    throw new Exception("Unsupported curve.");
            }
        }

        internal static void SetSecretKeyHeader(ECDsaCurveType curve, Span<byte> buffer)
        {
            switch (curve)
            {
                case ECDsaCurveType.P256:
                    Constants.MulticodecP256SecretKeyHeader.CopyTo(buffer);
                    break;
                case ECDsaCurveType.P384:
                    Constants.MulticodecP384SecretKeyHeader.CopyTo(buffer);
                    break;
                case ECDsaCurveType.P521:
                    Constants.MulticodecP521SecretKeyHeader.CopyTo(buffer);
                    break;
                default:
                    throw new Exception("Unsupported curve.");
            }
        }

        internal static void SetPublicKeyHeader(string? algorithm, Span<byte> buffer)
        {
            switch (algorithm)
            {
                case ECDsaCurve.P256:
                    Constants.MulticodecP256PublicKeyHeader.CopyTo(buffer);
                    break;
                case ECDsaCurve.P384:
                    Constants.MulticodecP384PublicKeyHeader.CopyTo(buffer);
                    break;
                case ECDsaCurve.P521:
                    Constants.MulticodecP521PublicKeyHeader.CopyTo(buffer);
                    break;
                default:
                    throw new Exception("Unsupported curve.");
            }
        }

        internal static void SetPublicKeyHeader(ECDsaCurveType curve, Span<byte> buffer)
        {
            switch (curve)
            {
                case ECDsaCurveType.P256:
                    Constants.MulticodecP256PublicKeyHeader.CopyTo(buffer);
                    break;
                case ECDsaCurveType.P384:
                    Constants.MulticodecP384PublicKeyHeader.CopyTo(buffer);
                    break;
                case ECDsaCurveType.P521:
                    Constants.MulticodecP521PublicKeyHeader.CopyTo(buffer);
                    break;
                default:
                    throw new Exception("Unsupported curve.");
            }
        }
    }
}
