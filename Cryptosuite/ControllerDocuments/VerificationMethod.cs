using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Cryptosuite.Core.ControllerDocuments
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class VerificationMethod : BaseDocument
    {
        public required string Type { get; set; }
        public string? Controller { get; set; }
        public DateTime? Revoked { get; set; }
    }
}