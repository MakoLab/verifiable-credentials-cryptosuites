using Cryptosuite.Core;
using FluentResults;
using JsonLdExtensions;
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
        public override object CreateProof(string document, ProofPurpose purpose, ProofSet proofSet, IDocumentLoader documentLoader)
        {
            throw new NotImplementedException();
        }

        public override object Derive(string document, ProofPurpose purpose, ProofSet proofSet, IDocumentLoader documentLoader)
        {
            throw new NotImplementedException();
        }

        public override Result VerifyProof(Proof proof, string document, ProofPurpose purpose, ProofSet proofSet, IDocumentLoader documentLoader)
        {
            throw new NotImplementedException();
        }
    }
}
