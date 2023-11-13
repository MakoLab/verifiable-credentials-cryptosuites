using Cryptosuite.Core;
using Cryptosuite.Core.Interfaces;
using ECDsa_Multikey;
using JsonLdExtensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF.JsonLd;

namespace ECDsa_2019_Cryptosuite
{
    public class ECDsa2019Cryptosuite : ICryptosuite, ICanonize
    {
        public string RequiredAlgorithm { get { return "P-256"; } }

        public string Name { get { return "ecdsa-2019"; } }

        public Verifier CreateVerifier(VerificationMethod verificationMethod)
        {
            if (verificationMethod is not MultikeyModel)
            {
                throw new Exception("VerificationMethod must be a MultikeyModel");
            }
            var key = Multikey.From((MultikeyModel)verificationMethod);
            return key.Verifier;
        }

        public string Canonize(JToken input, JsonLdNormalizerOptions options)
        {
            return JsonLdNormalizer.Normalize(input, options);
        }
    }
}
