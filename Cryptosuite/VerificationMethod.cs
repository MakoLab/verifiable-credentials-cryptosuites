using Cryptosuite.Core.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cryptosuite.Core
{
    public class VerificationMethod
    {
        [JsonProperty("@context")]
        public JToken? Context { get; set; }
        public string? Id { get; set; }
        public string? Type { get; set; }
        public string? Controller { get; set; }
        public IEnumerable<VerificationMethod>? AssertionMethod { get; set; }
    }
}