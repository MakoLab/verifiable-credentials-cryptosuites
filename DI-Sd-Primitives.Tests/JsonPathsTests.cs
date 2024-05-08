using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI_Sd_Primitives.Tests
{
    public class JsonPathsTests
    {
        [Fact]
        public void JsonPointerToPaths_ReturnsPaths()
        {
            // Arrange
            var jsonPointer = "/a/b/c";

            // Act
            var paths = JsonSelectionExtension.JsonPointerToPaths(jsonPointer);

            // Assert
            Assert.Equal(3, paths.Count);
            Assert.Equal("a", paths[0].AsT0);
            Assert.Equal("b", paths[1].AsT0);
            Assert.Equal("c", paths[2].AsT0);
        }

        [Fact]
        public void JsonPointerToPaths_ReturnsPaths_WithEscapedCharacters()
        {
            // Arrange
            var jsonPointer = "/a~1b/c~0d";

            // Act
            var paths = JsonSelectionExtension.JsonPointerToPaths(jsonPointer);

            // Assert
            Assert.Equal(2, paths.Count);
            Assert.Equal("a/b", paths[0].AsT0);
            Assert.Equal("c~d", paths[1].AsT0);
        }

        [Fact]
        public void JsonPointerToPaths_ReturnsPaths_WithArrayIndex()
        {
            // Arrange
            var jsonPointer = "/a/1/c";

            // Act
            var paths = JsonSelectionExtension.JsonPointerToPaths(jsonPointer);

            // Assert
            Assert.Equal(3, paths.Count);
            Assert.Equal("a", paths[0].AsT0);
            Assert.Equal(1, paths[1].AsT1);
            Assert.Equal("c", paths[2].AsT0);
        }
    }
}
