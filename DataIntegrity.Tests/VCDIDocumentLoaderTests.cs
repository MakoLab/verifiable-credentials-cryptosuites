using Cryptosuite.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataIntegrity.Tests
{
    public class VCDIDocumentLoaderTests
    {
        [Fact]
        public void LoadDocumentWithStaticHttpScheme()
        {
            // Arrange
            var documentLoader = new VCDIDocumentLoader(new DidDocumentCreator());
            var url = new Uri(Contexts.DataIntegrityV1Url);

            // Act
            var document = documentLoader.LoadDocument(url);

            // Assert
            Assert.NotNull(document);
            Assert.NotNull(document.Document);
            Assert.Equal(url, document.DocumentUrl);
        }

        [Fact]
        public void LoadDocumentWithDidKeyScheme()
        {
            // Arrange
            var documentLoader = new VCDIDocumentLoader(new DidDocumentCreator());
            var url = new Uri("did:key:zDnaepBuvsQ8cpsWrVKw8fbpGpvPeNSjVPTWoq6cRqaYzBKVP");

            // Act
            var document = documentLoader.LoadDocument(url);

            // Assert
            Assert.NotNull(document);
            Assert.NotNull(document.Document);
            Assert.Equal(url, document.DocumentUrl);
        }
    }
}
