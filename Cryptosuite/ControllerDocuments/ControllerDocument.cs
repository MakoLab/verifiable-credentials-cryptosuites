using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptosuite.Core.ControllerDocuments
{
    public class ControllerDocument
    {
        [JsonProperty("@context")]
        public JToken? Context { get; set; }
        public string Id { get; set; }
        public string? Controller { get; set; }
        public IEnumerable<VerificationMethod> VerificationMethod { get; set; }

        public ControllerDocument(string id)
        {
            Id = id;
            VerificationMethod = new List<VerificationMethod>();
        }
    }
}
