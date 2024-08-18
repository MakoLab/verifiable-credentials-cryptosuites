using JsonLdExtensions;
using Newtonsoft.Json.Linq;

namespace Cryptosuite.Core.Interfaces
{
    public interface ICreateProofValue
    {
        public string CreateProofValue(byte[] verifyData, JObject document, Proof proof, IEnumerable<Proof> proofSet, IDocumentLoader documentLoader, Signer signer);
    }
}
