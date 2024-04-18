using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using System.Text.RegularExpressions;
using JsonLdExtensions.Canonicalization;
using JsonLdExtensions;
using VDS.RDF.Writing.Formatting;

namespace DI_Sd_Primitives
{
    public class SkolemizationService
    {
        public List<string> Skolemize(List<string> nQuads, string urnScheme)
        {
            var ts = new TripleStore();
            ts.LoadFromString(string.Join("\n", nQuads));
            var sts = new TripleStore();
            foreach (var quad in ts.GetQuads())
            {
                var nodes = new List<INode>();
                foreach (var node in quad.Components)
                {
                    nodes.Add(SkolemizeNode(node, urnScheme));
                }
                sts.Add(new Quad(nodes[0], nodes[1], nodes[2], nodes[3]));
            }
            var formatter = new NQuadsCanonFormatter();
            return sts.GetQuads().Select(q => q.ToNQuad(formatter)).ToList();
        }

        public List<string> Deskolemize(List<string> nQuads, string urnScheme)
        {
            var ts = new TripleStore();
            ts.LoadFromString(string.Join("\n", nQuads));
            var sts = new TripleStore();
            foreach (var quad in ts.GetQuads())
            {
                var s = DeskolemizeNode(quad.Subject, urnScheme);
                var o = DeskolemizeNode(quad.Object, urnScheme);
                var g = DeskolemizeNode(quad.Graph.Name, urnScheme);
                sts.Add(new Quad(g, s, quad.Predicate, o));
            }
            var formatter = new NQuadsCanonFormatter();
            return sts.GetQuads().Select(q => q.ToNQuad(formatter)).ToList();
        }

        private static INode SkolemizeNode(INode node, string urnScheme)
        {
            return node switch
            {
                IBlankNode blankNode => new UriNode(new Uri($"urn:{urnScheme}:{blankNode.InternalID}")),
                _ => node
            };
        }

        private static INode DeskolemizeNode(INode node, string urnScheme)
        {
            return node switch
            {
                IUriNode uriNode => uriNode.Uri.AbsoluteUri.StartsWith($"urn:{urnScheme}:")
                                    ? new BlankNode(uriNode.Uri.AbsoluteUri[$"urn:{urnScheme}:".Length..])
                                    : uriNode,
                _ => node
            };
        }

        private static string Skolemize(string nQuad, string replacement, Regex regex)
        {
            return regex.Replace(nQuad, replacement);
        }

        private static string Deskolemize(string nQuad, string replacement, Regex regex)
        {
            return regex.Replace(nQuad, replacement);
        }

        //Regex matching blank nodes, with exception for blank nodes in literals
        //[GeneratedRegex(@"(_:([^\s]+))(?=([^""\\]*(\\.|""([^""\\]*\\.)*[^""\\]*""))*[^""]*$)", RegexOptions.Compiled)]
        //private static partial Regex SkolemizeRegex();
    }
}
