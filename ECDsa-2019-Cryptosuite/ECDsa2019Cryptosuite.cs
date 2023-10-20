using Cryptosuite;
using JsonLD.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDsa_2019_Cryptosuite
{
    public class ECDsa2019Cryptosuite : ICryptosuite
    {
        public string RequiredAlgorithm { get { return "P-256"; } }

        public string Name { get { return "ecdsa-2019"; } }

        public Verifier CreateVerifier(string publicKey)
        {
            throw new NotImplementedException();
        }

        public string Canonize(JToken input, JsonLdOptions options)
        {
            options.format = "application/n-quads";
            return (string)JsonLdProcessor.Normalize(input, options);
        }
    }
}
