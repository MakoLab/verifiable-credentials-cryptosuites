﻿using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;

namespace Cryptosuite.Core
{
    public enum ECDsaCurveType
    {
        P256,
        P384,
        P521
    }

    public static class ECDsaCurve
    {
        public const string P256 = "P-256";
        public const string P384 = "P-384";
        public const string P521 = "P-521";

        public static X9ECParameters ToECCurve(string curve)
        {
            return ECNamedCurveTable.GetByName(curve);
        }

        public static DerObjectIdentifier ToDerObjectIdentifier(ECDsaCurveType curve)
        {
            return curve switch
            {
                ECDsaCurveType.P256 => X9ObjectIdentifiers.ECDsaWithSha256,
                ECDsaCurveType.P384 => X9ObjectIdentifiers.ECDsaWithSha384,
                ECDsaCurveType.P521 => X9ObjectIdentifiers.ECDsaWithSha512,
                _ => throw new NotSupportedException($"Curve {curve} is not supported."),
            };
        }

        public static string ToAlgorithmName(this DerObjectIdentifier id)
        {
            if (id.Id == X9ObjectIdentifiers.ECDsaWithSha256.Id)
            {
                return P256;
            }
            if (id.Id == X9ObjectIdentifiers.ECDsaWithSha384.Id)
            {
                return P384;
            }
            if (id.Id == X9ObjectIdentifiers.ECDsaWithSha512.Id)
            {
                return P521;
            }
            throw new NotSupportedException($"Curve {id.Id} is not supported.");
        }

        public static ECDsaCurveType ToECDsaCurveType(string curve)
        {
            return curve switch
            {
                P256 => ECDsaCurveType.P256,
                P384 => ECDsaCurveType.P384,
                P521 => ECDsaCurveType.P521,
                _ => throw new NotSupportedException($"Curve {curve} is not supported."),
            };
        }

        public static IDigest GetDigest(ECDsaCurveType curve)
        {
            return curve switch
            {
                ECDsaCurveType.P256 => new Sha256Digest(),
                ECDsaCurveType.P384 => new Sha384Digest(),
                ECDsaCurveType.P521 => new Sha512Digest(),
                _ => throw new NotSupportedException($"Curve {curve} is not supported."),
            };
        }
    }
}
