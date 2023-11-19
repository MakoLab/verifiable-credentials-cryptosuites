using JsonLdExtensions;
using JsonLdSignatures.Purposes;
using JsonLdSignatures.Suites;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonLdSignatures
{
    public class ProofSetService
    {
        public JObject Add(JObject document, LinkedDataSignature suite, ProofPurpose purpose, IDocumentLoader documentLoader)
        {
            throw new NotImplementedException();
        }
    }
}
