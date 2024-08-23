using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Cryptosuite.Core.ControllerDocuments
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class VerificationMethod
    {
        [JsonProperty("@context")]
        public JToken? Context { get; set; }
        public required string Id { get; set; }
        public required string Type { get; set; }
        public required string Controller { get; set; }
        public DateTime? Revoked { get; set; }
    }
}