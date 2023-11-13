using JsonLdExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptosuite.Core.Interfaces
{
    public interface ICreateVerifyData
    {
        public byte[] CreateVerifyData(string document, Proof proof, ProofSet proofSet, IDocumentLoader documentLoader);
    }
}
