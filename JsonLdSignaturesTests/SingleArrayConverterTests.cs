using Cryptosuite.Core.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cryptosuite.Core.Tests
{
    public class SingleArrayConverterTests
    {
        [Fact]
        public void ShouldSerializeCount1ListAsProperty()
        {
            var myObject = new MyObject
            {
                Name = "Test",
                Items = new List<string> { "Item1" }
            };
            var serialized = JsonConvert.SerializeObject(myObject);
            Assert.Equal("""{"Name":"Test","Items":"Item1"}""", serialized);
        }

        [Fact]
        public void ShouldSerializeCount2ListAsArray()
        {
            var myObject = new MyObject
            {
                Name = "Test",
                Items = new List<string> { "Item1", "Item2" }
            };
            var serialized = JsonConvert.SerializeObject(myObject);
            Assert.Equal("""{"Name":"Test","Items":["Item1","Item2"]}""", serialized);
        }

        [Fact]
        public void ShouldNotSerializeNullValue()
        {
            var myObject = new MyObject
            {
                Name = null,
                Items = new List<string> { "Item1", "Item2" }
            };
            var serialized = JsonConvert.SerializeObject(myObject);
            Assert.Equal("""{"Items":["Item1","Item2"]}""", serialized);
        }

        [Fact]
        public void ShouldNotSerializeNullList()
        {
            var myObject = new MyObject
            {
                Name = "Test",
                Items = null
            };
            var serialized = JsonConvert.SerializeObject(myObject);
            Assert.Equal("""{"Name":"Test"}""", serialized);
        }

        [Fact]
        public void ShouldDeserializeCount1ArrayAsList()
        {
            var serialized = """{"Name":"Test","Items":"Item1"}""";
            var myObject = JsonConvert.DeserializeObject<MyObject>(serialized);
            Assert.NotNull(myObject);
            Assert.Equal("Test", myObject.Name);
            Assert.Equal("Item1", myObject.Items?.First());
        }

        [Fact]
        public void ShouldParseCount1ListAsProperty()
        {
            var serialized = """{"Name":"Test","Items":"Item1"}""";
            var jObject = JObject.Parse(serialized);
            var myObject = jObject.ToObject<MyObject>();
            Assert.NotNull(myObject);
            Assert.Equal("Test", myObject.Name);
            Assert.Equal("Item1", myObject.Items?.First());
        }

        [Fact]
        public void ShouldParseCount2ArrayAsList()
        {
            var serialized = """{"Name":"Test","Items":["Item1","Item2"]}""";
            var jObject = JObject.Parse(serialized);
            var myObject = jObject.ToObject<MyObject>();
            Assert.NotNull(myObject);
            Assert.Equal("Test", myObject.Name);
            Assert.Equal("Item1", myObject.Items?.First());
        }

        [Fact]
        public void ShouldParseCustomObjectWithSerializer()
        {
            var serialized = """{"Name":"Test","@context":"Item1"}""";
            var jObject = JObject.Parse(serialized);
            if (jObject["@context"] is not null)
            {
                var serializer = new JsonSerializer();
                serializer.Converters.Add(new SingleArrayConverter<string>());
                var context = jObject["@context"]!.ToObject<IEnumerable<string>>(serializer);
                Assert.NotNull(context);
                Assert.Equal("Item1", context.First());
            }
        }

        [Fact]
        public void ShouldParseAsNull()
        {
            var serialized = """{"Name":"Test"}""";
            var jObject = JObject.Parse(serialized);
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new SingleArrayConverter<string>());
            var context = jObject["@context"]?.ToObject<IEnumerable<string>>(serializer);
            Assert.Null(context);
        }
    }

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class MyObject
    {
        public string? Name { get; set; }
        [JsonConverter(typeof(SingleArrayConverter<string>))]
        public IEnumerable<string>? Items { get; set; }
    }
}
