using JsonLdExtensions;
using Newtonsoft.Json.Linq;

namespace Cryptosuite.Core.Interfaces
{
    public interface ICreateVerifyData
    {
        public byte[] CreateVerifyData(JObject document, Proof proof, IEnumerable<Proof> proofSet, IDocumentLoader documentLoader);
    }
}
