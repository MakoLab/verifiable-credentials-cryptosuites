using Cryptosuite.Core.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Cryptosuite.Core
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Proof
    {
        [JsonProperty("id")]
        public string? Id { get; set; }
        [JsonProperty("@context")]
        public JToken? Context { get; set; }
        [JsonProperty("type")]
        public string? Type { get; set; }
        [JsonProperty("created")]
        public DateTime? Created { get; set; }
        [JsonProperty("expires")]
        public DateTime? Expires { get; set; }
        [JsonProperty("proofPurpose")]
        public string? ProofPurpose { get; set; }
        [JsonProperty("proofValue")]
        public string? ProofValue { get; set; }
        [JsonProperty("verificationMethod")]
        public string? VerificationMethod { get; set; }
        [JsonProperty("domain")]
        public IEnumerable<string>? Domain { get; set; }
        [JsonProperty("challenge")]
        public string? Challenge { get; set; }
        [JsonProperty("previousProof")]
        public string? PreviousProof { get; set; }
        [JsonProperty("nonce")]
        public string? Nonce { get; set; }
        [JsonProperty("cryptosuite")]
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
            Context = proof.Context;
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
