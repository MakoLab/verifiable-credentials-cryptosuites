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
        [Fact]
        public void SelectPaths_ReturnsSelectedPaths()
        {
            // Arrange
            var document = JObject.Parse("""
                    {
                        "@context": {
                            "@vocab": "http://example.org/"
                        },
                        "urn:example_subject": {
                            "http://example.org/property1": "value1",
                            "http://example.org/property2": "value2"
                        }
                    }
                """);
            var paths = new List<OneOf<string, int>> { "urn:example_subject", "http://example.org/property1" };
            var arrays = new List<JArray>();
            var selectionDocument = new JObject();

            // Act
            JsonSelectionExtension.SelectPaths(document, paths, selectionDocument, arrays);

            // Assert
            Assert.Equal("value1", selectionDocument["urn:example_subject"]?["http://example.org/property1"]);
        }
    }
}
