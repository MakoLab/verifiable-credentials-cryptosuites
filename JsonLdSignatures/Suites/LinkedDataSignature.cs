using Cryptosuite;
using JsonLD.Core;
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
        public override object CreateProof(string document, ProofPurpose purpose, ProofSet proofSet, DocumentLoader documentLoader)
        {
            throw new NotImplementedException();
        }

        public override object Derive(string document, ProofPurpose purpose, ProofSet proofSet, DocumentLoader documentLoader)
        {
            throw new NotImplementedException();
        }

        public override bool VerifyProof(Proof proof, string document, ProofPurpose purpose, ProofSet proofSet, DocumentLoader documentLoader)
        {
            throw new NotImplementedException();
        }
    }
}
