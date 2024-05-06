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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VDS.RDF.JsonLd;
using VDS.RDF.Writing;
using Microsoft.Json.Pointer;
using OneOf;

namespace DI_Sd_Primitives
{
    public static class SkolemizationService
    {
        public static List<string> Skolemize(List<string> nQuads, string urnScheme)
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

        public static List<string> Deskolemize(List<string> nQuads, string urnScheme)
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

        public static (JToken skolemizedExpandedDocument, JToken skolemizedCompactDocument) SkolemizeCompactJsonLd(JToken compactJson, string urnScheme)
        {
            var context = compactJson["@context"];
            var expandedJson = JsonLdProcessor.Expand(compactJson);
            var count = 0;
            var skolemizedExpandedDocument = SkolemizeExpandedJsonLd(expandedJson, urnScheme, Guid.NewGuid().ToString(), ref count);
            var skolemizedCompactDocument = JsonLdProcessor.Compact(skolemizedExpandedDocument, context, new JsonLdProcessorOptions());
            return (skolemizedExpandedDocument, skolemizedCompactDocument);
        }

        public static List<string> ToDeskolemizedNQuads(this JToken skolemizedDocument)
        {
            var triplestore = new TripleStore();
            triplestore.LoadFromString(skolemizedDocument.ToString());
            var nQuadsWriter = new NQuadsWriter();
            var data = VDS.RDF.Writing.StringWriter.Write(triplestore, nQuadsWriter);
            var nQuads = data.Split("\n").ToList();
            var deskolemizedNQuads = Deskolemize(nQuads, "custom-scheme");
            return deskolemizedNQuads;
        }

        /// <summary>
        /// Converts a JSON Pointer to an array of paths into a JSON tree.
        /// </summary>
        /// <param name="jsonPointer">JSON Pointer string.</param>
        /// <returns>An array of paths.</returns>
        public static List<OneOf<string, int>> JsonPointerToPaths(string jsonPointer)
        {
            var paths = new List<OneOf<string, int>>();
            var splitPath = jsonPointer.Split('/')[..1];
            foreach (var path in splitPath)
            {
                if (path.Contains('~'))
                    paths.Add(path.UnescapeJsonPointer());
                else
                    paths.Add(Int32.TryParse(path, out var number) ? number : path);
            }
            return paths;
        }

        /// <summary>
        /// Creates an initial selection (a fragment of a JSON-LD document) based on a JSON-LD object.
        /// </summary>
        /// <param name="source">JSON-LD object.</param>
        /// <returns>JSON-LD document fragment object.</returns>
        public static JObject CreateInitialSelection(JObject source)
        {
            var selection = new JObject();
            var id = source["id"];
            if (id is not null && id.Type == JTokenType.String)
            {
                var idString = id.ToString();
                if (!idString.StartsWith("_:"))
                {
                    selection.Add("id", idString);
                }
            }
            if (source["type"] is not null)
            {
                selection.Add(source["type"]);
            }
            return selection;
        }

        /// <summary>
        /// Selects a portion of a compact JSON-LD document using paths parsed from a parsed JSON Pointer.
        /// </summary>
        /// <param name="paths">An array of paths parsed from a JSON Pointer.</param>
        /// <param name="document">A compact JSON-LD document.</param>
        /// <param name="selectionDocument">A selection document to be populated.</param>
        /// <param name="arrays">An array of arrays for tracking selected arrays.</param>
        public static void SelectPaths(List<OneOf<string, int>> paths, JObject document, JObject selectionDocument, List<JArray> arrays)
        {
            JToken? parentValue = document;
            JToken? value = parentValue;
            JToken? selectedParent = selectionDocument;
            JToken? selectedValue = selectedParent;
            OneOf<string, int> lastPath = default;
            foreach (var path in paths)
            {
                lastPath = path;
                selectedParent = selectedValue;
                parentValue = value;
                value = parentValue.GetValue(path);
                if (value is null)
                {
                    throw new ArgumentException("JSON pointer does not match the given document.");
                }
                selectedValue = selectedParent[path];
                if (selectedValue is null)
                {
                    if (value.Type == JTokenType.Array)
                    {
                        selectedValue = new JArray();
                        arrays.Add((JArray)selectedValue);
                    }
                    else
                    {
                        selectedValue = CreateInitialSelection((JObject)value);
                    }
                    ((JObject)selectedParent).Add(selectedValue);
                }
            }
            if (value is JValue)
            {
                selectedValue = value;
            }
            else if (value is JArray)
            {
                selectedValue = new JArray(value);
            }
            else
            {
                selectedValue = new JObject(selectedValue);
                ((JObject)selectedValue).Merge((JObject)value);
            }
            selectedParent[lastPath] = selectedValue;
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

        private static JToken? GetValue(this JToken jToken, OneOf<string, int> key)
        {
            if (key.IsT0)
            {
                return jToken[key.AsT0];
            }
            if (key.IsT1)
            {
                if (jToken.Type == JTokenType.Array)
                {
                    return ((JArray)jToken)[key.AsT1];
                }
                return null;
            }
            return null;
        }
    }
}
