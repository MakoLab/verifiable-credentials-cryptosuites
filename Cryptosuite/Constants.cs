using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Cryptosuite.Core
{
    public static class ECDsaCurve
    {
        public const string P256 = "P-256";
        public const string P384 = "P-384";
        public const string P521 = "P-521";

        public static ECCurve ToECCurve(string curve)
        {
            return curve switch
            {
                ECDsaCurve.P256 => ECCurve.NamedCurves.nistP256,
                ECDsaCurve.P384 => ECCurve.NamedCurves.nistP384,
                ECDsaCurve.P521 => ECCurve.NamedCurves.nistP521,
                _ => throw new Exception($"Unsupported curve: {curve}"),
            };
        }

        public static string FromECCurve(ECCurve curve)
        {
            return curve.Oid.FriendlyName switch
            {
                "nistP256" => ECDsaCurve.P256,
                "nistP384" => ECDsaCurve.P384,
                "nistP521" => ECDsaCurve.P521,
                _ => throw new Exception($"Unsupported curve: {curve.Oid.FriendlyName}"),
            };
        }
    }
}
