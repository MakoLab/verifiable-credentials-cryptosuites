using JsonLdExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptosuite.Core.Interfaces
{
    public interface ICreateProofValue
    {
        public string CreateProofValue(byte[] verifyData, string document, Proof proof, ProofSet proofSet, IDocumentLoader documentLoader);
    }
}
