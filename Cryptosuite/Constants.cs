using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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

        public static DerObjectIdentifier ToDerObjectIdentifier(string curve)
        {
            return curve switch
            {
                P256 => X9ObjectIdentifiers.ECDsaWithSha256,
                P384 => X9ObjectIdentifiers.ECDsaWithSha384,
                P521 => X9ObjectIdentifiers.ECDsaWithSha512,
                _ => throw new NotSupportedException($"Curve {curve} is not supported."),
            };
        }

        public static string ToAlgorithmName(this DerObjectIdentifier id)
        {
            if (id.Id == X9ObjectIdentifiers.ECDsaWithSha256.Id)
            {
                return ECDsaCurve.P256;
            }
            if (id.Id == X9ObjectIdentifiers.ECDsaWithSha384.Id)
            {
                return ECDsaCurve.P384;
            }
            if (id.Id == X9ObjectIdentifiers.ECDsaWithSha512.Id)
            {
                return ECDsaCurve.P521;
            }
            throw new NotSupportedException($"Curve {id.Id} is not supported.");
        }
    }
}
