﻿using Cryptosuite;
using JsonLD.Core;
using JsonLdSignatures.Purposes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public abstract object CreateProof(string document, ProofPurpose purpose, ProofSet proofSet, DocumentLoader documentLoader);

        public abstract object Derive(string document, ProofPurpose purpose, ProofSet proofSet, DocumentLoader documentLoader);

        public abstract bool VerifyProof(Proof proof, string document, ProofPurpose purpose, ProofSet proofSet, DocumentLoader documentLoader);

        public bool MatchProof(Proof proof)
        {
            return proof.Type == Type;
        }
    }
}
