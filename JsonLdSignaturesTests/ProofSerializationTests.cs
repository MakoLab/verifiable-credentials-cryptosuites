using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptosuite.Core.Tests
{
    public class ProofSerializationTests
    {
        [Fact]
        public void ProofSerialization()
        {
            var proof = new Proof
            {
                Id = "https://example.com/proofs/3732",
                Context = new JValue("https://w3id.org/security/v2"),
                Type = "Ed25519Signature2018",
                Created = DateTime.Parse("2019-12-11T03:50:55Z"),
                Expires = DateTime.Parse("2020-12-11T03:50:55Z"),
                ProofPurpose = "assertionMethod",
            };
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };
            var json = JsonConvert.SerializeObject(proof, settings);
            Assert.NotNull(json);
            var expected = """
                {
                  "id": "https://example.com/proofs/3732",
                  "@context": "https://w3id.org/security/v2",
                  "type": "Ed25519Signature2018",
                  "created": "2019-12-11T03:50:55Z",
                  "expires": "2020-12-11T03:50:55Z",
                  "proofPurpose": "assertionMethod"
                }
                """;
            Assert.Equal(expected, json);
        }

        [Fact]
        public void ProofDeserialization()
        {
            var json = """
                {
                  "@context": "https://w3id.org/security/v2",
                  "type": "DataIntegrityProof",
                  "cryptosuite": "jcs-eddsa-2022",
                  "created": "2023-03-05T19:23:24Z",
                  "verificationMethod": "https://di.example/issuer#z6MkjLrk3gKS2nnkeWcmcxiZPGskmesDpuwRBorgHxUXfxnG",
                  "proofPurpose": "assertionMethod",
                  "proofValue": "zQeVbY4oey5q2M3XKaxup3tmzN4DRFTLVqpLMweBrSxMY2xHX5XTYV8nQApmEcqaqA3Q1gVHMrXFkXJeV6doDwLWx"
                }
                """;
            var proof = JsonConvert.DeserializeObject<Proof>(json);
            Assert.NotNull(proof);
            Assert.Equal("https://w3id.org/security/v2", proof.Context);
            Assert.Equal("DataIntegrityProof", proof.Type);
            Assert.Equal("jcs-eddsa-2022", proof.CryptoSuiteName);
            Assert.Equal(DateTime.Parse("2023-03-05T19:23:24Z").ToUniversalTime(), proof.Created);
            Assert.Equal("https://di.example/issuer#z6MkjLrk3gKS2nnkeWcmcxiZPGskmesDpuwRBorgHxUXfxnG", proof.VerificationMethod);
            Assert.Equal("assertionMethod", proof.ProofPurpose);
            Assert.Equal("zQeVbY4oey5q2M3XKaxup3tmzN4DRFTLVqpLMweBrSxMY2xHX5XTYV8nQApmEcqaqA3Q1gVHMrXFkXJeV6doDwLWx", proof.ProofValue);
        }

        [Fact]
        public void ProofDeserializationWithArrayContext()
        {
            var json = """
              {
                "@context": [ "https://w3id.org/security/v2", "https://w3id.org/security/v3" ],
                  "type": "DataIntegrityProof"
              }
              """;
            var proof = JsonConvert.DeserializeObject<Proof>(json);
            Assert.NotNull(proof);
            Assert.NotNull(proof.Context);
            Assert.Equal(JTokenType.Array, proof.Context.Type);
            Assert.Equal(2, proof.Context.Count());
            Assert.Equal("https://w3id.org/security/v2", proof.Context[0]);
            Assert.Equal("https://w3id.org/security/v3", proof.Context[1]);
            Assert.Equal("DataIntegrityProof", proof.Type);
        }
    }
}
