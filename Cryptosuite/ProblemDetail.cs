using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Cryptosuite.Core
{
    /// <summary>
    /// Represents a problem detail as defined in RFC 9457.
    /// </summary>
    public class ProblemDetail
    {
        [JsonProperty("type")]
        public required Uri Type { get; set; }
        [JsonProperty("title")]
        public required string Title { get; set; }
        [JsonProperty("detail")]
        public required string Detail { get; set; }
        [JsonProperty("status")]
        public HttpStatusCode? Status { get; set; }
    }
}
