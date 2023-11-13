using JsonLdExtensions;
using SecurityDocumentLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ECDsa_2019_Cryptosuite.Tests
{
    public class ECDsa2019CryptosuiteTests
    {
        readonly JsonLdDocumentLoader documentLoader = new SecurityDocumentLoader.SecurityDocumentLoader();
        readonly MockData mockData = new();

        [Fact]
        public void ShouldCanonizeDocument()
        {
            var expected = """
                <http://example.edu/credentials/1872> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://schema.org#AlumniCredential> .
                <http://example.edu/credentials/1872> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://www.w3.org/2018/credentials#VerifiableCredential> .
                <http://example.edu/credentials/1872> <https://www.w3.org/2018/credentials#credentialSubject> <https://example.edu/students/alice> .
                <http://example.edu/credentials/1872> <https://www.w3.org/2018/credentials#issuanceDate> "2010-01-01T19:23:24Z"^^<http://www.w3.org/2001/XMLSchema#dateTime> .
                <http://example.edu/credentials/1872> <https://www.w3.org/2018/credentials#issuer> <https://example.edu/issuers/565049> .
                <https://example.edu/students/alice> <https://schema.org#alumniOf> "Example University" .\n
                """;
            var credential = mockData.Credential;
            var cryptosuite = new ECDsa2019Cryptosuite();
            var canonized = cryptosuite.Canonize(credential, new JsonLdNormalizerOptions() { DocumentLoader = documentLoader.LoadDocument });
            Assert.Equal(expected, canonized);
        }
    }
}
