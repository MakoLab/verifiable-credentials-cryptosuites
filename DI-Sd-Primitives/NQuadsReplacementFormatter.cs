using JsonLdExtensions.Canonicalization;
using VDS.RDF;
using VDS.RDF.Writing;

namespace DI_Sd_Primitives
{
    internal class NQuadsReplacementFormatter : NQuadsCanonFormatter
    {
        private readonly IDictionary<string, string> _labelMap;
        internal NQuadsReplacementFormatter(IDictionary<string, string> labelMap)
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
