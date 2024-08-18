using Cryptosuite.Core.Util;
using Newtonsoft.Json;

namespace Cryptosuite.Core.Tests
{
    public class ObjectIdToStringConverterTests
    {
        [Fact]
        public void ShouldConvertStringToString()
        {
            var json = """{"method":"Id_value","type":"TestId"}""";
            var obj = JsonConvert.DeserializeObject<ObjectIdToStringObject>(json);
            Assert.NotNull(obj);
            Assert.Equal("Id_value", obj.Method);
            Assert.Equal("TestId", obj.Type);
        }

        [Fact]
        public void ShouldConvertObjectIdToString()
        {
            var json = """{"method":{"id":"Id_value", "someProperty":"whatever"},"type":"TestId"}""";
            var obj = JsonConvert.DeserializeObject<ObjectIdToStringObject>(json);
            Assert.NotNull(obj);
            Assert.Equal("Id_value", obj.Method);
            Assert.Equal("TestId", obj.Type);
        }
    }

    public class ObjectIdToStringObject
    {
        [JsonConverter(typeof(ObjectIdToStringConverter))]
        public string? Method { get; set; }
        public string? Type { get; set; }
    }
}
