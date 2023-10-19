using JsonLdSignatures.Purposes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonLdSignatures.Suites
{
    internal class LinkedDataSignature : LinkedDataProof
    {
        public LinkedDataSignature(string type) : base(type)
        {
        }
        public override object CreateProof(Document document, ProofPurpose purpose, ProofSet proofSet, DocumentLoader documentLoader)
        {
            throw new NotImplementedException();
        }

        public override object Derive(Document document, ProofPurpose purpose, ProofSet proofSet, DocumentLoader documentLoader)
        {
            throw new NotImplementedException();
        }

        public override bool VerifyProof(Proof proof, Document document, ProofPurpose purpose, ProofSet proofSet, DocumentLoader documentLoader)
        {
            throw new NotImplementedException();
        }
    }
}
