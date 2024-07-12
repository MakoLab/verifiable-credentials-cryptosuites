using JsonLdExtensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptosuite.Core.Interfaces
{
    public interface ICreateProofValue
    {
        public string CreateProofValue(byte[] verifyData, JObject document, Proof proof, IEnumerable<Proof> proofSet, IDocumentLoader documentLoader, Signer signer);
    }
}
