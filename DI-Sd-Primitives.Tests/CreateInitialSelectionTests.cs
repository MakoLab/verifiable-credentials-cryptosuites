using Newtonsoft.Json.Linq;

namespace DI_Sd_Primitives.Tests
{
    public class CreateInitialSelectionTests
    {
        [Fact]
        public void CreateInitialSelection_ReturnsInitialSelection()
        {
            // Arrange
            var source = JObject.Parse(@"
            {
                ""id"": ""urn:example_subject"",
                ""type"": ""http://example.org/Type""
            }");

            // Act
            var selection = source.CreateInitialSelection();

            // Assert
            Assert.Equal("urn:example_subject", selection["id"]);
            Assert.Equal("http://example.org/Type", selection["type"]);
        }

        [Fact]
        public void CreateInitialSelection_ReturnsInitialSelection_WithoutId()
        {
            // Arrange
            var source = JObject.Parse(@"
            {
                ""type"": ""http://example.org/Type""
            }");

            // Act
            var selection = source.CreateInitialSelection();

            // Assert
            Assert.Null(selection["id"]);
            Assert.Equal("http://example.org/Type", selection["type"]);
        }

        [Fact]
        public void CreateInitialSelection_ReturnsInitialSelection_WithoutType()
        {
            // Arrange
            var source = JObject.Parse(@"
            {
                ""id"": ""urn:example_subject""
            }");

            // Act
            var selection = source.CreateInitialSelection();

            // Assert
            Assert.Equal("urn:example_subject", selection["id"]);
            Assert.Null(selection["type"]);
        }

        [Fact]
        public void CreateInitialSelection_ReturnsInitialSelection_WithoutIdAndType()
        {
            // Arrange
            var source = JObject.Parse("{}");

            // Act
            var selection = source.CreateInitialSelection();

            // Assert
            Assert.Null(selection["id"]);
            Assert.Null(selection["type"]);
        }

        [Fact]
        public void CreateInitialSelection_ReturnsInitialSelection_WithBlankNodeIdentifier()
        {
            // Arrange
            var source = JObject.Parse(@"
            {
                ""id"": ""_:subject"",
                ""type"": ""http://example.org/Type""
            }");

            // Act
            var selection = source.CreateInitialSelection();

            // Assert
            Assert.Null(selection["id"]);
            Assert.Equal("http://example.org/Type", selection["type"]);
        }
    }
}
