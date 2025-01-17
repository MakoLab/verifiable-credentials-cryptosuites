using JsonLdExtensions.Canonicalization;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using JsonLdExtensions;
using VDS.RDF;
using VDS.RDF.JsonLd;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;

namespace DI_Sd_Primitives
{
    public static class SkolemizationService
    {
        public static IList<string> Skolemize(IList<string> nQuads, string urnScheme)
        {
            var ts = new TripleStore();
            ts.LoadFromString(string.Join("\n", nQuads));
            var formatter = new NQuadsCanonFormatter();
            var list = new List<string>();
            foreach (var quad in ts.Quads)
            {
                var sQuad = new Quad(
                    SkolemizeNode(quad.Subject, urnScheme)!,
                    SkolemizeNode(quad.Predicate, urnScheme)!,
                    SkolemizeNode(quad.Object, urnScheme)!,
                    SkolemizeNode(quad.Graph, urnScheme) as IRefNode
                    );
                list.Add(sQuad.ToNQuad(formatter));
            }
            return list;
        }

        public static IList<string> Deskolemize(IList<string> nQuads, string urnScheme)
        {
            var ts = new TripleStore();
            ts.LoadFromString(string.Join("\n", nQuads));
            var formatter = new NQuadsCanonFormatter();
            var list = new List<string>();
            foreach (var quad in ts.Quads)
            {
                var s = DeskolemizeNode(quad.Subject, urnScheme)!;
                var o = DeskolemizeNode(quad.Object, urnScheme)!;
                var g = DeskolemizeNode(quad.Graph, urnScheme) as IRefNode;
                var dQuad = (new Quad(s, quad.Predicate, o, g));
                list.Add(dQuad.ToNQuad(formatter));
            }
            return list;
        }

        /// <summary>
        /// Replaces all blank node identifiers in an expanded JSON-LD document with custom-scheme URNs, including assigning such URNs to blank nodes that are unlabeled.
        /// </summary>
        /// <param name="expandedJson">Expanded JSON-LD document.</param>
        /// <param name="urnScheme">Custom URN scheme.</param>
        /// <param name="guid">UUID string.</param>
        /// <param name="count">Shared integer counter.</param>
        /// <returns>Skolemized expanded document.</returns>
        public static JArray SkolemizeExpandedJsonLd(JArray expandedJson, string urnScheme, string guid, ref int count)
        {
            var skolemizedExpandedDocument = new JArray();
            foreach (var token in expandedJson)
            {
                //If either element is not an object or it contains the key @value, append a copy of element to
                //skolemizedExpandedDocument and continue to the next element.
                if (token.Type != JTokenType.Object || token["@value"] != null)
                {
                    skolemizedExpandedDocument.Add(token);
                    continue;
                }
                //Otherwise, initialize skolemizedNode to an object, and for each property and value in element:
                var skolemizedNode = new JObject();
                foreach (var (key, value) in (JObject)token)
                {
                    //If value is an array, set the value of property in skolemizedNode to the result of calling this
                    //algorithm recursively passing value for expanded and keeping the other parameters the same.
                    if (value is JArray array)
                    {
                        skolemizedNode.Add(key, SkolemizeExpandedJsonLd(array, urnScheme, guid, ref count));
                    }
                    //Otherwise, set the value of property in skolemizedNode to the first element in the array result
                    //of calling this algorithm recursively passing an array with value as its only element for
                    //expanded and keeping the other parameters the same.
                    else
                    {
                        skolemizedNode.Add(key, SkolemizeExpandedJsonLd(new JArray(value!), urnScheme, guid, ref count).First());
                    }
                }
                //If skolemizedNode has no @id property, set the value of the @id property in skolemizedNode to the
                //concatenation of "urn:", urnScheme, "_", randomString, "_" and the value of count, incrementing the
                //value of count afterwards.
                if (skolemizedNode["@id"] is null)
                {
                    skolemizedNode.Add("@id", $"urn:{urnScheme}_{guid}_{count++}");
                }
                //Otherwise, if the value of the @id property in skolemizedNode starts with "_:", preserve the existing
                //blank node identifier when skolemizing by setting the value of the @id property in skolemizedNode to
                //the concatenation of "urn:", urnScheme, and the blank node identifier (i.e., the existing value of
                //the @id property minus the "_:" prefix; e.g., if the existing value of the @id property is _:b0, the
                //blank node identifier is b0).
                else if (skolemizedNode["@id"]!.ToString().StartsWith("_:"))
                {
                    skolemizedNode["@id"] = $"urn:{urnScheme}_{skolemizedNode["@id"]!.ToString()[2..]}";
                }
                //Append skolemizedNode to skolemizedExpandedDocument.
                skolemizedExpandedDocument.Add(skolemizedNode);
            }
            return skolemizedExpandedDocument;
        }

        public static (JArray skolemizedExpandedDocument, JObject skolemizedCompactDocument) SkolemizeCompactJsonLd(JToken compactJson, string urnScheme)
        {
            var context = compactJson["@context"];
            var processorWarnings = new List<JsonLdProcessorWarning>();
            var expandedJson = JsonLdProcessor.Expand(compactJson, new JsonLdProcessorOptions() { RemoteContextLimit = -1 }, processorWarnings);
            if (processorWarnings.Any())
            {
                throw new DataLossException(String.Join("\n", processorWarnings.Select(w => w.Message)));
            }
            var count = 0;
            var skolemizedExpandedDocument = SkolemizeExpandedJsonLd(expandedJson, urnScheme, Guid.NewGuid().ToString(), ref count);
            var skolemizedCompactDocument = JsonLdProcessor.Compact(skolemizedExpandedDocument, context, new JsonLdProcessorOptions());
            return (skolemizedExpandedDocument, skolemizedCompactDocument);
        }

        public static IList<string> ToDeskolemizedNQuads(this JToken skolemizedDocument, string customUrnScheme)
        {
            var triplestore = new TripleStore();
            triplestore.SafeLoadFromString(skolemizedDocument.ToString(), new JsonLdParser());
            var nQuadsWriter = new NQuadsWriter();
            var data = VDS.RDF.Writing.StringWriter.Write(triplestore, nQuadsWriter);
            var nQuads = data.Split("\n").ToList();
            var deskolemizedNQuads = Deskolemize(nQuads, customUrnScheme);
            return deskolemizedNQuads;
        }

        public static IList<string> RelabelBlankNodes(IList<string> nQuads, IDictionary<string, string> labelMap)
        {
            var formatter = new NQuadsReplacementFormatter(labelMap);
            var ts = new TripleStore();
            ts.LoadFromString(string.Join("\n", nQuads));
            var result = new List<string>();
            foreach (var quad in ts.GetQuads())
            {
                result.Add(quad.ToNQuad(formatter));
            }
            return result;
        }

        private static INode? SkolemizeNode(INode? node, string urnScheme)
        {
            return node switch
            {
                IBlankNode blankNode => new UriNode(new Uri($"urn:{urnScheme}_{blankNode.InternalID}")),
                _ => node
            };
        }

        private static INode? DeskolemizeNode(INode? node, string urnScheme)
        {
            return node switch
            {
                IUriNode uriNode => uriNode.Uri.AbsoluteUri.StartsWith($"urn:{urnScheme}_")
                                    ? new BlankNode(uriNode.Uri.AbsoluteUri[$"urn:{urnScheme}_".Length..])
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
    }
}
