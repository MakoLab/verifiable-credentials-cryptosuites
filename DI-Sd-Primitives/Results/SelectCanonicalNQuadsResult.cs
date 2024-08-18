using Newtonsoft.Json.Linq;

namespace DI_Sd_Primitives.Results
{
    public class SelectCanonicalNQuadsResult
    {
        public required JObject SelectionDocument { get; init; }
        public required IList<string> DeskolemizedNQuads { get; init; }
        public required IList<string> NQuads { get; init; }
    }
}
