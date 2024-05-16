using DI_Sd_Primitives.Tests.InlineTestData;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI_Sd_Primitives.Tests
{
    public class SelectJsonLdTests
    {
        [Fact]
        public void SelectJsonLd_ReturnsJsonObject()
        {
            var document = JObject.Parse(SelectPaths_ReturnsSelectedPaths_TestData.CredentialJson);
            var pointers = new List<string>()
            {
                "/payload/description",
                "/payload/credentialSubject/givenName",
                "/payload/credentialSubject/familyName",
                "/payload/credentialSubject/alumniOf",
                "/payload/credentialBranding/backgroundColor",
                "/payload/type/1"
            };
            var selectionDocument = document.SelectJsonLd(pointers);
            Assert.NotNull(selectionDocument);
            Assert.Equal("Diploma in Management", selectionDocument.SelectToken("payload.description"));
            Assert.Equal("Emma", selectionDocument.SelectToken("payload.credentialSubject.givenName"));
            Assert.Equal("did:key:z6Mkr9f7o82NFLRFTTCWRR1GiZpca22Xf6YKo2zKThrZMA2w", selectionDocument.SelectToken("payload.credentialSubject.id"));
            Assert.Null(selectionDocument.SelectToken("payload.credentialSubject.type"));
            Assert.Equal("AlumniCredential", selectionDocument.SelectToken("payload.type[0]"));
        }
    }
}
