using Cryptosuite.Core.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using OneOf;

namespace Cryptosuite.Core.ControllerDocuments
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class ControllerDocument
    {
        [JsonProperty("@context")]
        public JToken? Context { get; set; }
        public string Id { get; set; }
        public string? Controller { get; set; }
        public IList<VerificationMethod>? VerificationMethod { get; set; }
        public IList<OneOf<string, VerificationMethod>>? Authentication { get; set; }
        [JsonProperty(ItemConverterType = typeof(OneOfConverter<string, VerificationMethod>))]
        public IList<OneOf<string, VerificationMethod>>? AssertionMethod { get; set; }

        public ControllerDocument(string id)
        {
            Id = id;
        }
    }
}
