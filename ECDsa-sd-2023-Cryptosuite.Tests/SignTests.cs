using DataIntegrity;
using ECDsa_Multikey;
using JsonLdSignatures;
using JsonLdSignatures.Purposes;
using Newtonsoft.Json.Linq;

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
            var keypair = MultikeyService.From(new MultikeyVerificationMethod()
            {
                Id = MockData.VerificationMethodId,
                Controller = MockData.ControllerId,
                PublicKeyMultibase = MockData.PublicKeyMultibase,
                SecretKeyMultibase = MockData.SecretKeyMultibase,
            });
            var date = DateTime.Parse("2023-03-01T21:29:24Z");
            var crypto = new ECDsaSd2023CreateProofCryptosuite();
            var suite = new DataIntegrityProof(crypto, keypair.Signer, date);
            var jsonLd = new JsonLdSignatureService();
            var loader = new SecurityDocumentLoader.SecurityDocumentLoader();
            var purpose = new AssertionMethodPurpose(new Cryptosuite.Core.Controller { Id = MockData.VerificationMethodId }, date);
            var signed = suite.CreateProof(document, purpose, [], loader);
            Assert.NotNull(signed);
        }
    }
}
