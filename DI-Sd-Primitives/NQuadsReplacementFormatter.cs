using DI_Sd_Primitives.Interfaces;
using JsonLdExtensions.Canonicalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Writing;

namespace DI_Sd_Primitives
{
    internal class NQuadsReplacementFormatter : NQuadsCanonFormatter
    {
        private readonly Dictionary<string, string> _labelMap;
        internal NQuadsReplacementFormatter(Dictionary<string, string> labelMap)
            : base(preserveOriginalUriString: true)
        {
            _labelMap = labelMap;
        }

        protected override string FormatBlankNode(IBlankNode bNode, TripleSegment? segment)
        {
            if (_labelMap.TryGetValue(bNode.InternalID, out string? value))
            {
                bNode = new BlankNode(value);
            }
            return base.FormatBlankNode(bNode, segment);
        }
    }
}
