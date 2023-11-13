using JsonLdExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptosuite.Core.Interfaces
{
    public interface IDerive
    {
        public string Derive(string document, IProofPurpose proofPurpose, ProofSet proofSet, IDocumentLoader documentLoader);
    }
}
