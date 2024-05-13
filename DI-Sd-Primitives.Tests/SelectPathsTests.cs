using DI_Sd_Primitives.Tests.InlineTestData;
using Newtonsoft.Json.Linq;
using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
