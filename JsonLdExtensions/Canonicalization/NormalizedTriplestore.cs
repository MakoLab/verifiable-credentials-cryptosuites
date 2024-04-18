using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Writing.Formatting;

namespace JsonLdExtensions.Canonicalization
{
    public class NormalizedTriplestore
    {
        public TripleStore Store { get; private set; }
        public OrderedDictionary IssuedIdentifiers { get; private set; }

        public NormalizedTriplestore(TripleStore store, OrderedDictionary identifierIssuerMap)
        {
            Store = store;
            IssuedIdentifiers = identifierIssuerMap;
        }

        public string Serialize()
        {
            return String.Join("", SerializeToList());
        }

        public string Serialize(NQuadsCanonFormatter formatter)
        {
            return String.Join("", SerializeToList(formatter));
        }

        public List<string> SerializeToList()
        {
            var formatter = new NQuadsCanonFormatter(preserveOriginalUriString: true);
            var lines = new List<string>();
            foreach (var quad in Store.GetQuads())
            {
                lines.Add(formatter.Format(quad.Triple, quad.Graph.Name));
            }
            lines.Sort();
            return lines;
        }

        public string SerializeToList(NQuadsCanonFormatter formatter)
        {
            var lines = new List<string>();
            foreach (var quad in Store.GetQuads())
            {
                lines.Add(formatter.Format(quad.Triple, quad.Graph.Name));
            }
            lines.Sort();
            return String.Join("", lines);
        }
    }
}
