using DI_Sd_Primitives.Tests.InlineTestData;
using Newtonsoft.Json.Linq;

namespace DI_Sd_Primitives.Tests
{
    public class SelectPathsTests
    {
        [Theory]
        [ClassData(typeof(SelectPaths_ReturnsSelectedPaths_TestData))]
        public void SelectPaths_ReturnsSelectedPaths(string data, string path, string expected)
        {
            // Arrange
            var document = JObject.Parse(data);
            var paths = JsonSelectionExtension.JsonPointerToPaths(path);
            var arrays = new List<JArray>();
            var selectionDocument = new JObject();

            // Act
            JsonSelectionExtension.SelectPaths(document, paths, selectionDocument, arrays);

            // Assert
            Assert.Equal(expected, selectionDocument.SelectToken(JsonSelectionExtension.JsonPointerToJsonPath(path)));
        }

        [Fact]
        public void SelectPaths_ReturnsIdAndType()
        {
            // Arrange
            var document = JObject.Parse(SelectPaths_ReturnsSelectedPaths_TestData.CredentialJson);
            var paths = JsonSelectionExtension.JsonPointerToPaths("/payload/issuer/name");
            var arrays = new List<JArray>();
            var selectionDocument = new JObject();

            // Act
            JsonSelectionExtension.SelectPaths(document, paths, selectionDocument, arrays);

            // Assert
            Assert.Equal("Zealopia Business Institute", selectionDocument.SelectToken("payload.issuer.name"));
            Assert.Equal("did:key:z6Mkr9f7o82NFLRFTTCWRR1GiZpca22Xf6YKo2zKThrZMA2w", selectionDocument.SelectToken("payload.issuer.id"));
            Assert.Equal(2, Assert.IsType<JArray>(selectionDocument.SelectToken("payload.type")).Count);
            Assert.Equal("EducationalOccupationalCredential", selectionDocument.SelectToken("payload.type[0]"));
        }
    }
}
