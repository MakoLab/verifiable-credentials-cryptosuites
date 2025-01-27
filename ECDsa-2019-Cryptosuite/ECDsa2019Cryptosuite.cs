﻿using Cryptosuite.Core;
using Cryptosuite.Core.ControllerDocuments;
using Cryptosuite.Core.Interfaces;
using ECDsa_Multikey;
using JsonLdExtensions.Canonicalization;
using Newtonsoft.Json.Linq;

namespace ECDsa_2019_Cryptosuite
{
    public class ECDsa2019Cryptosuite : ICryptosuite, ICanonize, ICreateVerifier
    {
        public string RequiredAlgorithm { get => "P-256"; }
        public string Name { get => "ecdsa-rdfc-2019"; }
        public static string TypeName { get => "ecdsa-rdfc-2019"; }

        public Verifier CreateVerifier(VerificationMethod verificationMethod)
        {
            if (verificationMethod.Type?.ToLower() != "multikey")
            {
                throw new Exception("VerificationMethod must be a MultikeyModel");
            }
            var key = MultikeyService.From((MultikeyVerificationMethod)verificationMethod);
            return key.Verifier;
        }

        public string Canonize(JToken input, JsonLdNormalizerOptions options)
        {
            return JsonLdNormalizer.Normalize(input, options).SerializedNQuads.Replace("\r", String.Empty);
        }
    }
}
