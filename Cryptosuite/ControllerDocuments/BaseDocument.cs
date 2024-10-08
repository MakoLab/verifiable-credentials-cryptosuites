using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace Cryptosuite.Core.ControllerDocuments
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public abstract class BaseDocument
    {
        [JsonProperty("@context")]
        public JToken? Context { get; set; }
        public string? Id { get; set; }
    }
}
