using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI_Sd_Primitives.Results
{
    public class GroupResult
    {
        public required Dictionary<int, string> Matching { get; set; }
        public required Dictionary<int, string> NonMatching { get; set; }
        public required List<string> DeskolemizedNQuads { get; set; }
    }
}
