using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.JsonLd;
using VDS.RDF.JsonLd.Processors;
using VDS.RDF.JsonLd.Syntax;
using VDS.RDF.Parsing;
using VDS.RDF.Parsing.Handlers;

namespace JsonLdExtensions
{
    public class SafeJsonLdParser : JsonLdParser, IStoreReader
    {
        private const string XsdNs = "http://www.w3.org/2001/XMLSchema#";
        private const string RdfNs = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
        private static readonly Regex ExponentialFormatMatcher = new(@"(\d)0*E\+?0*");

        public event StoreReaderWarning? SafeWarning;

        public SafeJsonLdParser() : base()
        {
        }

        public SafeJsonLdParser(JsonLdProcessorOptions? options) : base(options)
        {
        }

        /// <inheritdoc/>
        public new void Load(ITripleStore store, TextReader input)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
            if (input == null) throw new ArgumentNullException(nameof(input));
            Load(new StoreHandler(store), input, store.UriFactory);
        }

        /// <inheritdoc />
        public new void Load(IRdfHandler handler, TextReader input, IUriFactory uriFactory)
        {
            JToken element;
            using (var reader = new JsonTextReader(input) { DateParseHandling = DateParseHandling.None })
            {
                element = JToken.ReadFrom(reader);
            }
            JArray expandedElement = JsonLdProcessor.Expand(element, ParserOptions);
            Load(handler, expandedElement, uriFactory);
        }

        private void Load(IRdfHandler handler, JArray input, IUriFactory uriFactory)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (uriFactory == null) throw new ArgumentNullException(nameof(uriFactory));

            handler.StartRdf();
            IUriNode rdfTypeNode = handler.CreateUriNode(uriFactory.Create(RdfNs + "type"));
            try
            {
                var nodeMapGenerator = new NodeMapGenerator();
                JObject nodeMap = nodeMapGenerator.GenerateNodeMap(input);
                foreach (JProperty p in nodeMap.Properties())
                {
                    var graphName = p.Name;
                    if (p.Value is not JObject graph) continue;
                    IRefNode? graphNode;
                    if (graphName == "@default")
                    {
                        graphNode = null;
                    }
                    else
                    {
                        if (IsBlankNodeIdentifier(graphName))
                        {
                            graphNode = handler.CreateBlankNode(graphName[2..]);
                        }
                        else if (Uri.TryCreate(graphName, UriKind.Absolute, out Uri graphIri) && graphIri.IsWellFormedOriginalString())
                        {
                            graphNode = handler.CreateUriNode(graphIri);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    foreach (JProperty gp in graph.Properties())
                    {
                        var subject = gp.Name;
                        var node = gp.Value as JObject;
                        IRefNode subjectNode;
                        if (IsBlankNodeIdentifier(subject))
                        {
                            subjectNode = handler.CreateBlankNode(subject[2..]);
                        }
                        else
                        {
                            if (!(Uri.TryCreate(subject, UriKind.Absolute, out Uri subjectIri) &&
                                  subjectIri.IsWellFormedOriginalString()))
                            {
                                RaiseWarning(
                                    $"Unable to generate a well-formed absolute IRI for subject `{subjectIri}`. This subject will be ignored.");
                                continue;
                            }
                            subjectNode = handler.CreateUriNode(subjectIri);
                        }
                        foreach (JProperty np in node.Properties())
                        {
                            var property = np.Name;
                            var values = np.Value as JArray;
                            if (property.Equals("@type"))
                            {
                                foreach (JToken type in values)
                                {
                                    INode typeNode = MakeNode(handler, type, graphNode);
                                    if (typeNode is null)
                                    {
                                        RaiseWarning(
                                            $"Unable to generate a well-formed absolute IRI for type `{type}`. This type will be ignored.");
                                        continue;
                                    }
                                    handler.HandleQuad(new Triple(subjectNode, rdfTypeNode, typeNode), graphNode);
                                }
                            }
                            else if ((IsBlankNodeIdentifier(property) && ParserOptions.ProduceGeneralizedRdf) ||
                                     Uri.IsWellFormedUriString(property, UriKind.Absolute))
                            {
                                foreach (JToken item in values)
                                {
                                    var predicateNode = MakeNode(handler, property, graphNode) as IRefNode;
                                    INode objectNode = MakeNode(handler, item, graphNode);
                                    if (predicateNode is null)
                                    {
                                        RaiseWarning(
                                            $"Unable to generate a well-formed absolute IRI for predicate `{property}`. This property will be ignored.");
                                        continue;
                                    }
                                    if (objectNode is null)
                                    {
                                        RaiseWarning(
                                            $"Unable to generate a well-formed absolute IRI for object `{item}`. This property will be ignored.");
                                        continue;
                                    }
                                    handler.HandleQuad(new Triple(subjectNode, predicateNode, objectNode), graphNode);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                handler.EndRdf(false);
                throw;
            }
            handler.EndRdf(true);
        }

        private INode MakeNode(IRdfHandler handler, JToken token, IRefNode graphName, bool allowRelativeIri = false)
        {
            if (token is JValue)
            {
                var stringValue = token.Value<string>();
                if (IsBlankNodeIdentifier(stringValue))
                {
                    return handler.CreateBlankNode(stringValue[2..]);
                }
                if (Uri.TryCreate(stringValue, allowRelativeIri ? UriKind.RelativeOrAbsolute : UriKind.Absolute, out Uri iri))
                {
                    if (!Uri.IsWellFormedUriString(stringValue, allowRelativeIri ? UriKind.RelativeOrAbsolute : UriKind.Absolute)) return null;
                    return handler.CreateUriNode(iri);
                }
                return null;
            }

            if (IsValueObject(token) && token is JObject valueObject)
            {
                string literalValue;
                JToken value = valueObject["@value"];
                var datatype = valueObject.Property("@type")?.Value.Value<string>();
                var language = valueObject.Property("@language")?.Value.Value<string>();
                if (datatype == "@json")
                {
                    datatype = RdfNs + "JSON";
                    var serializer = new JsonLiteralSerializer();
                    literalValue = serializer.Serialize(value);
                }
                else if (value.Type == JTokenType.Boolean)
                {
                    literalValue = value.Value<bool>() ? "true" : "false";
                    datatype ??= XsdNs + "boolean";
                }
                else if (value.Type == JTokenType.Float ||
                         value.Type == JTokenType.Integer && datatype != null && datatype.Equals(XsdNs + "double"))
                {
                    var doubleValue = value.Value<double>();
                    var roundedValue = Math.Round(doubleValue);
                    if (doubleValue.Equals(roundedValue) && doubleValue < 1e21 && datatype == null)
                    {
                        // Integer values up to 10^21 should be rendered as an integer rather than a float
                        literalValue = roundedValue.ToString("F0");
                        // The JSON-LD test suite requires no leading minus sign when the value is 0
                        if (literalValue.Equals("-0")) literalValue = "0";
                        datatype = XsdNs + "integer";
                    }
                    else
                    {
                        literalValue = value.Value<double>().ToString("E15", CultureInfo.InvariantCulture);
                        literalValue = ExponentialFormatMatcher.Replace(literalValue, "$1E");
                        if (literalValue.EndsWith("E")) literalValue = literalValue + "0";
                        datatype ??= XsdNs + "double";
                    }
                }
                else if (value.Type == JTokenType.Integer ||
                         value.Type == JTokenType.Float && datatype != null && datatype.Equals(XsdNs + "integer"))
                {
                    literalValue = value.Value<long>().ToString("D", CultureInfo.InvariantCulture);
                    datatype ??= XsdNs + "integer";
                }
                else if (valueObject.ContainsKey("@direction") && ParserOptions.RdfDirection.HasValue)
                {
                    literalValue = value.Value<string>();
                    var direction = valueObject["@direction"].Value<string>();
                    language = valueObject.ContainsKey("@language")
                        ? valueObject["@language"].Value<string>().ToLowerInvariant()
                        : string.Empty;
                    if (ParserOptions.RdfDirection == JsonLdRdfDirectionMode.I18NDatatype)
                    {
                        datatype = "https://www.w3.org/ns/i18n#" + language + "_" + direction;
                        return handler.CreateLiteralNode(literalValue, new Uri(datatype));
                    }
                    // Otherwise direction mode is CompoundLiteral
                    IBlankNode literalNode = handler.CreateBlankNode();
                    Uri xsdString =
                        UriFactory.Root.Create(XmlSpecsHelper.XmlSchemaDataTypeString);
                    handler.HandleQuad(new Triple(
                        literalNode,
                        handler.CreateUriNode(UriFactory.Root.Create(RdfSpecsHelper.RdfValue)),
                        handler.CreateLiteralNode(literalValue, xsdString)),
                        graphName);
                    if (!string.IsNullOrEmpty(language))
                    {
                        handler.HandleQuad(new Triple(
                            literalNode,
                            handler.CreateUriNode(UriFactory.Root.Create(RdfSpecsHelper.RdfLanguage)),
                            handler.CreateLiteralNode(language, xsdString)),
                            graphName);
                    }

                    handler.HandleQuad(new Triple(
                        literalNode,
                        handler.CreateUriNode(UriFactory.Root.Create(RdfSpecsHelper.RdfDirection)),
                        handler.CreateLiteralNode(direction, xsdString)),
                        graphName);

                    return literalNode;
                }
                else
                {
                    literalValue = value.Value<string>();
                    if (datatype == null && language == null)
                    {
                        datatype = XsdNs + "string";
                    }
                }

                if (language != null)
                {
                    return LanguageTag.IsWellFormed(language) ? handler.CreateLiteralNode(literalValue, language) : null;
                }
                return handler.CreateLiteralNode(literalValue, new Uri(datatype));
            }
            if (IsListObject(token))
            {
                var listArray = token["@list"] as JArray;
                return MakeRdfList(handler, listArray, graphName);
            }

            if ((token as JObject)?.Property("@id") != null)
            {
                // Must be a node object
                var nodeObject = (JObject)token;
                return MakeNode(handler, nodeObject["@id"], graphName);
            }

            return null;
        }

        private INode MakeRdfList(IRdfHandler handler, JArray list, IRefNode graphName)
        {
            IUriNode rdfFirst = handler.CreateUriNode(new Uri(RdfNs + "first"));
            IUriNode rdfRest = handler.CreateUriNode(new Uri(RdfNs + "rest"));
            IUriNode rdfNil = handler.CreateUriNode(new Uri(RdfNs + "nil"));
            if (list == null || list.Count == 0) return rdfNil;
            var bNodes = list.Select(x => handler.CreateBlankNode()).ToList();
            for (var ix = 0; ix < list.Count; ix++)
            {
                IBlankNode subject = bNodes[ix];
                INode obj = MakeNode(handler, list[ix], graphName);
                if (obj != null)
                {
                    handler.HandleQuad(new Triple(subject, rdfFirst, obj), graphName);
                }
                INode rest = (ix + 1 < list.Count) ? bNodes[ix + 1] : (INode)rdfNil;
                handler.HandleQuad(new Triple(subject, rdfRest, rest), graphName);
            }
            return bNodes[0];
        }

        /// <summary>
        /// Determine if a JSON token is a JSON-LD value object.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>True of <paramref name="token"/> is a <see cref="JObject"/> with a. <code>@value</code> property, false otherwise.</returns>
        public static bool IsValueObject(JToken token)
        {
            return ((token as JObject)?.Property("@value")) != null;
        }

        /// <summary>
        /// Determine if a JSON token is a JSON-LD list object.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>True of <paramref name="token"/> is a <see cref="JObject"/> with a. <code>@list</code> property, false otherwise.</returns>
        public static bool IsListObject(JToken token)
        {
            return ((token as JObject)?.Property("@list")) != null;
        }

        /// <summary>
        /// Determine if the specified string is a blank node identifier.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool IsBlankNodeIdentifier(string? value)
        {
            return value != null && value.StartsWith("_:");
        }

        private void RaiseWarning(string message)
        {
            SafeWarning?.Invoke(message);
        }
    }
}
