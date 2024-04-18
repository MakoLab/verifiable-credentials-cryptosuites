using Cryptosuite.Core.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Cryptosuite.Core
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class VerificationMethod
    {
        [JsonProperty("@context")]
        public JToken? Context { get; set; }
        public string? Id { get; set; }
        public string? Type { get; set; }
        public string? Controller { get; set; }
        public IEnumerable<VerificationMethod>? AssertionMethod { get; set; }

        public virtual VerificationMethod? FromJson(string json)
        {
            return JsonConvert.DeserializeObject<VerificationMethod>(json);
        }
    }
}