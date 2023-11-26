using Cryptosuite.Core;
using FluentResults;
using JsonLdExtensions;
using JsonLdSignatures.Purposes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonLdSignatures.Suites
{
    public class LinkedDataSignature : LinkedDataProof
    {
        public LinkedDataSignature(string type) : base(type)
        {
        }
        public override Proof CreateProof(JObject document, ProofPurpose purpose, IEnumerable<Proof> proofSet, IDocumentLoader documentLoader)
        {
            throw new NotImplementedException();
        }

        public override JObject Derive(JObject document, ProofPurpose purpose, IEnumerable<Proof> proofSet, IDocumentLoader documentLoader)
        {
            throw new NotImplementedException();
        }

        public override Result<VerificationMethod> VerifyProof(Proof proof, JObject document, ProofPurpose purpose, IEnumerable<Proof> proofSet, IDocumentLoader documentLoader)
        {
            throw new NotImplementedException();
        }

        public void EnsureSuiteContext(JObject document, bool addSuiteContext)
        {
            throw new NotImplementedException();
        }
    }
}
