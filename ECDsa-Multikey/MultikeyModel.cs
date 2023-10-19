using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDsa_Multikey
{
    public class MultikeyModel
    {
        [JsonProperty("@context")]
        public IEnumerable<string>? Contexts { get; set; }
        [JsonProperty("type")]
        public string? Type { get; set; }
        [JsonProperty("id")]
        public string? Id { get; set; }
        [JsonProperty("controller")]
        
        public string? Controller { get; set; }
        [JsonProperty("revoked")]
        public DateTime? Revoked { get; set; }
        [JsonProperty("expires")]
        public DateTime? Expires { get; set; }
        [JsonProperty("publicKeyMultibase")]
        public string? PublicKeyMultibase { get; set; }
        [JsonProperty("secretKeyMultibase")]
        public string? SecretKeyMultibase { get; set; }
    }
}
