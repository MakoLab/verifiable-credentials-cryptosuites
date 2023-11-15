using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Writing.Formatting;

namespace JsonLdExtensions.Canonicalization
{
    public class NormalizedTriplestore
    {
        private readonly TripleStore _store;

        public NormalizedTriplestore(TripleStore store)
        {
            _store = store;
        }

        public string Serialize()
        {
            var formatter = new NQuadsCanonFormatter(preserveOriginalUriString: true);
            var lines = new List<string>();
            foreach (var quad in _store.GetQuads())
            {
                lines.Add(formatter.Format(quad.Triple, quad.Graph.Name));
            }
            lines.Sort();
            return string.Join("", lines);
        }
    }
}
