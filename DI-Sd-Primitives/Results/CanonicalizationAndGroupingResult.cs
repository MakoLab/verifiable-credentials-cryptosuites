using Newtonsoft.Json.Linq;

namespace DI_Sd_Primitives.Results
{
    public class CanonicalizationAndGroupingResult
    {
        public required IDictionary<string, GroupResult> Groups { get; set; }
        public required JObject SkolemizedCompactDocument { get; set; }
        public required JArray SkolemizedExpandedDocument { get; set; }
        public required IList<string> DeskolemizedNQuads { get; set; }
        public required IDictionary<string, string> LabelMap { get; set; }
        public required IList<string> CanonicalNQuads { get; set; }
    }
}
