using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonLdSignatures
{
    public class Proof
    {
        public string? Id { get; set; }
        public string? Type { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Expires { get; set; }
        public string? ProofPurpose { get; set; }
        public string? ProofValue { get; set; }
        public string? VerificationMethod { get; set; }
        public IEnumerable<string>? Domain { get; set; }
        public string? Challenge { get; set; }
        public string? PreviousProof { get; set; }
        public string? Nonce { get; set; }
        public string? CryptoSuiteName { get; set; }

        public Proof()
        {
        }

        /// <summary>
        /// Shallow copy constructor
        /// </summary>
        /// <param name="proof">Proof to copy</param>
        public Proof(Proof proof)
        {
            Id = proof.Id;
            Type = proof.Type;
            Created = proof.Created;
            Expires = proof.Expires;
            ProofPurpose = proof.ProofPurpose;
            ProofValue = proof.ProofValue;
            VerificationMethod = proof.VerificationMethod;
            Domain = proof.Domain;
            Challenge = proof.Challenge;
            PreviousProof = proof.PreviousProof;
            Nonce = proof.Nonce;
            CryptoSuiteName = proof.CryptoSuiteName;
        }
    }
}
