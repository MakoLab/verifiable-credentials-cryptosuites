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
        public string RequiredAlgorithm => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public Verifier CreateVerifier(string publicKey)
        {
            throw new NotImplementedException();
        }

        public object Canonize(JToken input, JsonLdOptions options)
        {
            options.format = "application/n-quads";
            return JsonLdProcessor.Normalize(input, options);
        }
    }
}
