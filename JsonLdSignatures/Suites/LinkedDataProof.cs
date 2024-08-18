using Cryptosuite.Core;
using Cryptosuite.Core.ControllerDocuments;
using FluentResults;
using JsonLdExtensions;
using JsonLdSignatures.Purposes;
using Newtonsoft.Json.Linq;

namespace JsonLdSignatures.Suites
{
    public abstract class LinkedDataProof
    {
        public string Type { get; set; }

        protected LinkedDataProof(string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentNullException($"A {nameof(type)} must have a 'type'.");
            }
            this.Type = type;
        }

        public abstract Proof CreateProof(JObject document, ProofPurpose purpose, IEnumerable<Proof> proofSet, IDocumentLoader documentLoader);

        public abstract JObject Derive(JObject document, ProofPurpose purpose, IEnumerable<Proof> proofSet, IDocumentLoader documentLoader);

        public abstract Result<VerificationMethod> VerifyProof(Proof proof, JObject document, ProofPurpose purpose, IEnumerable<Proof> proofSet, IDocumentLoader documentLoader);

        public virtual bool MatchProof(Proof proof)
        {
            return proof.Type == Type;
        }
    }
}
