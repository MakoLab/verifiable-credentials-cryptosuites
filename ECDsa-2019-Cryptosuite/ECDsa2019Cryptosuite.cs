using Cryptosuite.Core;
using Cryptosuite.Core.Interfaces;
using ECDsa_Multikey;
using JsonLD.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public string Canonize(string input, object options)
        {
            if (options is JsonLdOptions opts)
            {
            }
            else
            {
                opts = new JsonLdOptions();
                foreach (var prop in options.GetType().GetProperties())
                {
                    //if prop.Name is in opts, set it
                    var propInfo = opts.GetType().GetProperty(prop.Name);
                    propInfo?.SetValue(opts, prop.GetValue(options));
                }
            }
            opts.format = "application/n-quads";
            return (string)JsonLdProcessor.Normalize(input, opts);
        }
    }
}
