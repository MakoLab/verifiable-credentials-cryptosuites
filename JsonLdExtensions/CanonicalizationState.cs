using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;

namespace JsonLdExtensions
{
    internal class CanonicalizationState
    {
        internal Dictionary<string, IList<Quad>> BlankNodeToQuads { get; set; } = new();
        internal Dictionary<string, IList<string>> HashToBlankNodeIdentifier { get; set; } = new();
        private IdentifierIssuer CanonicalIssuer { get; set; } = new(Constants.CanonicalPrefix);

        internal void AddQuadToBlankNode(string blankNode, string graph, Triple triple)
        {
            if (!BlankNodeToQuads.ContainsKey(blankNode))
            {
                BlankNodeToQuads.Add(blankNode, new List<Quad>());
            }
            BlankNodeToQuads[blankNode].Add(new Quad(new UriNode(UriFactory.Create(graph)), triple));
        }

        internal void AddQuadToBlankNode(string blankNode, IRefNode graph, Triple triple)
        {
            if (!BlankNodeToQuads.ContainsKey(blankNode))
            {
                BlankNodeToQuads.Add(blankNode, new List<Quad>());
            }
            BlankNodeToQuads[blankNode].Add(new Quad(graph, triple));
        }

        internal IList<Quad> GetQuadsForBlankNode(string blankNode)
        {
            if (BlankNodeToQuads.ContainsKey(blankNode))
            {
                return BlankNodeToQuads[blankNode];
            }
            return new List<Quad>();
        }

        internal string IssueIdentifier(string identifier)
        {
            return CanonicalIssuer.IssueIdentifier(identifier);
        }

        internal string? GetIdentifier(string identifier)
        {
            return CanonicalIssuer.GetIdentifier(identifier);
        }

        internal bool IsIssued(string identifier)
        {
            return CanonicalIssuer.IsIssued(identifier);
        }
    }
}
