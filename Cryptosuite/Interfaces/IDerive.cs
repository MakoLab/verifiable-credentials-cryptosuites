using JsonLdExtensions;
using Newtonsoft.Json.Linq;

namespace Cryptosuite.Core.Interfaces
{
    public interface IDerive
    {
        public JObject Derive(JObject document, IProofPurpose proofPurpose, IEnumerable<Proof> proofSet, IDocumentLoader documentLoader);
    }
}
