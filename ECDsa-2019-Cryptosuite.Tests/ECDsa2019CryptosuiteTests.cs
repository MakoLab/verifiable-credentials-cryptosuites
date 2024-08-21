using DataIntegrity;
using ECDsa_Multikey;
using JsonLdExtensions;
using JsonLdExtensions.Canonicalization;
using JsonLdSignatures;
using JsonLdSignatures.Purposes;
using Newtonsoft.Json.Linq;

namespace ECDsa_2019_Cryptosuite.Tests
{
    public class ECDsa2019CryptosuiteTests
    {
        readonly JsonLdDocumentLoader documentLoader = new SecurityDocumentLoader.SecurityDocumentLoader();
        readonly MockData mockData = new();

        [Fact]
        public void ShouldCanonizeDocument()
        {
            // Arrange
            var expected = """
                <http://example.edu/credentials/1872> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://schema.org#AlumniCredential> .
                <http://example.edu/credentials/1872> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://www.w3.org/2018/credentials#VerifiableCredential> .
                <http://example.edu/credentials/1872> <https://www.w3.org/2018/credentials#credentialSubject> <https://example.edu/students/alice> .
                <http://example.edu/credentials/1872> <https://www.w3.org/2018/credentials#issuanceDate> "2010-01-01T19:23:24Z"^^<http://www.w3.org/2001/XMLSchema#dateTime> .
                <http://example.edu/credentials/1872> <https://www.w3.org/2018/credentials#issuer> <https://example.edu/issuers/565049> .
                <https://example.edu/students/alice> <https://schema.org#alumniOf> "Example University" .

                """.Replace("\r", String.Empty);
            var credential = mockData.Credential;
            var cryptosuite = new ECDsa2019Cryptosuite();

            // Act
            var canonized = cryptosuite.Canonize(credential, new JsonLdNormalizerOptions() { DocumentLoader = documentLoader.LoadDocument });

            // Assert
            Assert.Equal(expected, canonized);
        }

        [Fact]
        public void ShouldSignDocument()
        {
            // Arrange
            documentLoader.AddStatic(mockData.ControllerDocEcdsaMultikey.Id, JObject.FromObject(mockData.ControllerDocEcdsaMultikey));
            documentLoader.AddStatic(mockData.ControllerDocEcdsaMultikey.AssertionMethod![0].AsT0, JObject.FromObject(mockData.ControllerDocEcdsaMultikey.VerificationMethod![0]));
            var credential = mockData.Credential;
            var cryptosuite = new ECDsa2019Cryptosuite();
            var keypair = MultikeyService.From(mockData.MockPublicEcdsaMultikey);
            var suite = new DataIntegrityProof(cryptosuite, keypair.Signer);
            var jss = new JsonLdSignatureService();
            var proofPurpose = new AssertionMethodPurpose();
            documentLoader.AddStatic(mockData.MockPublicEcdsaMultikey.Id, JObject.FromObject(mockData.ControllerDocEcdsaMultikey));

            // Act
            var signed = jss.Sign(credential, suite, proofPurpose, documentLoader);
            var verified = jss.Verify(signed, suite, proofPurpose, documentLoader);

            // Assert
            Assert.NotNull(signed);
            var v = Assert.Single(verified);
            Assert.True(v.IsSuccess);
        }
    }
}
