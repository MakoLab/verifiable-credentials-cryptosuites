﻿using Newtonsoft.Json.Linq;
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

    public class SecurityConstants
    {
        public const string Ed25519Signature2020ContextUrl = "https://w3id.org/security/suites/ed25519-2020/v1";
        public const string VeresOneContextV1Url = "https://w3id.org/veres-one/v1";
        public const string X25519KeyAgreement2020V1ContextUrl = "https://w3id.org/security/suites/x25519-2020/v1";
        public const string CredentialsContextV1Url = "https://www.w3.org/2018/credentials/v1";
        public const string DidContextV1Url = "https://www.w3.org/ns/did/v1";
        public const string DataIntegrityV1Url = "https://w3id.org/security/data-integrity/v1";
        public const string SecurityContextV1Url = "https://w3id.org/security/v1";
        public const string SecurityContextV2Url = "https://w3id.org/security/v2";
        public const string SecurityContextUrl = "https://w3id.org/security/v2";
    }
}
