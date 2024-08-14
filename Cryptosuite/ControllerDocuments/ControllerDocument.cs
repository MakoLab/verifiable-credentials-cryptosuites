using Cryptosuite.Core.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptosuite.Core.ControllerDocuments
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class ControllerDocument
    {
        [JsonProperty("@context")]
        public JToken? Context { get; set; }
        public string Id { get; set; }
        public string? Controller { get; set; }
        public IEnumerable<VerificationMethod>? VerificationMethod { get; set; }
        public IEnumerable<OneOf<string, VerificationMethod>>? Authentication { get; set; }
        [JsonProperty(ItemConverterType = typeof(OneOfConverter<string, VerificationMethod>))]
        public IEnumerable<OneOf<string, VerificationMethod>>? AssertionMethod { get; set; }
        
        public ControllerDocument(string id)
        {
            Id = id;
        }
    }
}
