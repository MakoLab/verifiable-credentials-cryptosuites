using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Cryptosuite.Core.Tests
{
    public class ProblemDetailsTests
    {
        [Fact]
        public void ProblemDetails_WhenCreatedWithAllProperties_SetsProperties()
        {
            // Arrange & Act
            var problemDetails = new ProblemDetail
            {
                Type = new Uri("https://example.com/error"),
                Title = "Error",
                Detail = "An error occurred",
                Status = HttpStatusCode.BadRequest,
            };
            // Assert
            Assert.Equal("https://example.com/error", problemDetails.Type.ToString());
            Assert.Equal("Error", problemDetails.Title);
            Assert.Equal("An error occurred", problemDetails.Detail);
            Assert.Equal(HttpStatusCode.BadRequest, problemDetails.Status);
        }

        [Fact]
        public void ProblemDetails_WhenSerializedToJson_CreatesValidJson()
        {
            // Arrange
            var problemDetails = new ProblemDetail
            {
                Type = new Uri("https://example.com/error"),
                Title = "Error",
                Detail = "An error occurred",
                Status = HttpStatusCode.BadRequest,
            };
            // Act
            var json = JObject.FromObject(problemDetails);
            // Assert
            Assert.True(json.ContainsKey("type"));
            Assert.True(json.ContainsKey("title"));
            Assert.True(json.ContainsKey("detail"));
            Assert.True(json.ContainsKey("status"));
            Assert.NotNull(json["type"]);
            Assert.NotNull(json["title"]);
            Assert.NotNull(json["detail"]);
            Assert.NotNull(json["status"]);
            Assert.Equal("https://example.com/error", json["type"]!.ToString());
            Assert.Equal("Error", json["title"]!.ToString());
            Assert.Equal("An error occurred", json["detail"]!.ToString());
            Assert.Equal((int)HttpStatusCode.BadRequest, (int)json["status"]!);
        }
    }
}
