using Cryptosuite.Core.Util;
using Newtonsoft.Json;

namespace Cryptosuite.Core
{
    public class VerificationMethod
    {
        [JsonProperty("@context")]
        [JsonConverter(typeof(SingleArrayConverter<string>))]
        public IEnumerable<string>? Contexts { get; set; }
        public string? Id { get; set; }
        public string? Type { get; set; }
        public string? Controller { get; set; }
        public IEnumerable<VerificationMethod>? AssertionMethod { get; set; }
    }
}