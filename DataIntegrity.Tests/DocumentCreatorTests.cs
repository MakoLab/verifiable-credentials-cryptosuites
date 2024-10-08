using Cryptosuite.Core.ControllerDocuments;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataIntegrity.Tests
{
    public class DocumentCreatorTests
    {
        [Fact]
        public void CreateDocumentWithMultikey()
        {
            // Arrange
            var keyId = "did:key:zDnaepBuvsQ8cpsWrVKw8fbpGpvPeNSjVPTWoq6cRqaYzBKVP";
            var documentCreator = new DidDocumentCreator();

            // Act
            var document = documentCreator.CreateControllerDocument(new Uri(keyId));

            // Assert
            Assert.NotNull(document);
            Assert.Equal("did:key:zDnaepBuvsQ8cpsWrVKw8fbpGpvPeNSjVPTWoq6cRqaYzBKVP", document.Id);
            Assert.IsType<ControllerDocument>(document);
            var controllerDocument = (ControllerDocument)document;
            Assert.NotNull(controllerDocument.VerificationMethod);
            Assert.Equal("did:key:zDnaepBuvsQ8cpsWrVKw8fbpGpvPeNSjVPTWoq6cRqaYzBKVP#zDnaepBuvsQ8cpsWrVKw8fbpGpvPeNSjVPTWoq6cRqaYzBKVP", controllerDocument.VerificationMethod[0].Id);
            Assert.NotNull(document.Context);
            Assert.NotNull(controllerDocument.Authentication);
            Assert.NotNull(controllerDocument.AssertionMethod);
            Assert.Equal(2, document.Context.Count());
            Assert.Single(controllerDocument.VerificationMethod);
            Assert.Single(controllerDocument.Authentication);
            Assert.Single(controllerDocument.AssertionMethod);
        }

        [Fact]
        public void CreateDocumentWithKeyId()
        {
            // Arrange
            var keyId = "did:key:zDnaepBuvsQ8cpsWrVKw8fbpGpvPeNSjVPTWoq6cRqaYzBKVP#zDnaepBuvsQ8cpsWrVKw8fbpGpvPeNSjVPTWoq6cRqaYzBKVP";
            var documentCreator = new DidDocumentCreator();

            // Act
            var document = documentCreator.CreateControllerDocument(new Uri(keyId));

            // Assert
            Assert.NotNull(document);
            Assert.Equal("did:key:zDnaepBuvsQ8cpsWrVKw8fbpGpvPeNSjVPTWoq6cRqaYzBKVP#zDnaepBuvsQ8cpsWrVKw8fbpGpvPeNSjVPTWoq6cRqaYzBKVP", document.Id);
            Assert.IsAssignableFrom<VerificationMethod>(document);
            Assert.NotNull(document.Context);
            Assert.True(document.Context.Type == JTokenType.String);
        }
    }
}
