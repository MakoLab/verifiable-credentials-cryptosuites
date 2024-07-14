using DataIntegrity;
using ECDsa_Multikey;
using JsonLdSignatures;
using JsonLdSignatures.Purposes;
using Newtonsoft.Json.Linq;
using SecurityDocumentLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDsa_sd_2023_Cryptosuite.Tests
{
    public class SignTests
    {
        [Fact]
        public void ShouldSignDocument()
        {
            var document = new JObject
            {
                { "id", "http://example.edu/credentials/1872" },
                { "type", new JArray
                    {
                        "AlumniCredential",
                        "VerifiableCredential"
                    }
                },
                { "credentialSubject", "https://example.edu/students/alice" },
                { "issuanceDate", "2010-01-01T19:23:24Z" },
                { "issuer", "https://example.edu/issuers/565049" }
            };
            var keypair = Multikey.From(new MultikeyModel
            {
                PublicKeyMultibase = MockData.PublicKeyMultibase,
                SecretKeyMultibase = MockData.SecretKeyMultibase,
                Controller = MockData.Controller
            });
            var date = DateTime.Parse("2023-03-01T21:29:24Z");
            var crypto = new ECDsaSd2023CreateProofCryptosuite();
            var suite = new DataIntegrityProof(crypto, keypair.Signer, date);
            var jsonLd = new JsonLdSignatureService();
            var loader = new SecurityDocumentLoader.SecurityDocumentLoader();
            var purpose = new AssertionMethodPurpose(new Cryptosuite.Core.Controller { Id = MockData.Id }, date);
            var signed = suite.CreateProof(document, purpose, [], loader);
            Assert.NotNull(signed);
        }
    }
}
