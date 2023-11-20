using JsonLdExtensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptosuite.Core.Interfaces
{
    public interface IDerive
    {
        public JObject Derive(JObject document, IProofPurpose proofPurpose, IEnumerable<Proof> proofSet, IDocumentLoader documentLoader);
    }
}
