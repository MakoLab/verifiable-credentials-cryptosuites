using VDS.RDF;
using VDS.RDF.Writing;
using VDS.RDF.Writing.Formatting;

namespace JsonLdExtensions.Canonicalization
{
    public class NQuadsCanonFormatter : NQuadsFormatter
    {
        private const string XsdString = "http://www.w3.org/2001/XMLSchema#string";
        private readonly bool _preserveOriginalUriString;

        public NQuadsCanonFormatter(bool preserveOriginalUriString = false)
            : base(VDS.RDF.Parsing.NQuadsSyntax.Rdf11)
        {
            _preserveOriginalUriString = preserveOriginalUriString;
        }

        public override string Format(Triple t, IRefNode? graph)
        {
            if (graph is null)
            {
                return base.Format(t) + '\n';
            }
            return base.Format(t, graph) + '\n';
        }

        public override string Format(Triple t)
        {
            return base.Format(t) + '\n';
        }

        public override string FormatUri(Uri u)
        {
            if (_preserveOriginalUriString)
            {
                return base.FormatUri(u.OriginalString);
            }
            return base.FormatUri(u);
        }

        protected override string FormatLiteralNode(ILiteralNode n, TripleSegment? segment)
        {
            if (n.DataType.AbsoluteUri == XsdString)
            {
                var node = new LiteralNode(n.Value, datatype: null);
                return base.FormatLiteralNode(node, segment);
            }
            return base.FormatLiteralNode(n, segment);
        }
    }
}
