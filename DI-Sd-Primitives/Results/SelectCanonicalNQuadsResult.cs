using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI_Sd_Primitives.Results
{
    public class SelectCanonicalNQuadsResult
    {
        public required JObject SelectionDocument { get; init; }
        public required IList<string> DeskolemizedNQuads { get; init; }
        public required IList<string> NQuads { get; init; }
    }
}
