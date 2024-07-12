using AngleSharp.Dom;
using DI_Sd_Primitives.Interfaces;
using DI_Sd_Primitives.Results;
using Microsoft.Json.Pointer;
using Newtonsoft.Json.Linq;
using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;

namespace DI_Sd_Primitives
{
    public static class JsonSelectionExtension
    {
        public const string CustomUrnScheme = "ecdsa-sd-2023";

        /// <summary>
        /// Converts a JSON Pointer to an array of paths into a JSON tree.
        /// </summary>
        /// <param name="jsonPointer">JSON Pointer string.</param>
        /// <returns>An array of paths.</returns>
        public static List<OneOf<string, int>> JsonPointerToPaths(string jsonPointer)
        {
            var paths = new List<OneOf<string, int>>();
            var splitPath = jsonPointer.Split('/')[1..];
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
        public static JObject CreateInitialSelection(this JObject source)
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
                selection.Add("type", source["type"]);
            }
            return selection;
        }

        /// <summary>
        /// Selects a portion of a compact JSON-LD document using paths parsed from a parsed JSON Pointer.
        /// </summary>
        /// <param name="document">A compact JSON-LD document.</param>
        /// <param name="paths">An array of paths parsed from a JSON Pointer.</param>
        /// <param name="selectionDocument">A selection document to be populated.</param>
        /// <param name="arrays">An array of arrays for tracking selected arrays.</param>
        public static void SelectPaths(JObject document, List<OneOf<string, int>> paths, JObject selectionDocument, List<JArray> arrays)
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
                selectedValue = selectedParent.GetValue(path);
                if (selectedValue is null)
                {
                    switch (value.Type)
                    {
                        case JTokenType.Array:
                            selectedValue = new JArray();
                            arrays.Add((JArray)selectedValue);
                            break;
                        case JTokenType.Object:
                            selectedValue = CreateInitialSelection((JObject)value);
                            break;
                        default:
                            selectedValue = value;
                            break;
                    }
                    ((JObject)selectedParent).SetValue(path, selectedValue);
                }
            }
            switch (value)
            {
                case JValue:
                    selectedValue = value;
                    break;
                case JArray:
                    selectedValue = new JArray(value);
                    break;
                default:
                    selectedValue = new JObject(selectedValue);
                    ((JObject)selectedValue).Merge((JObject)value);
                    break;
            }
            selectedParent.SetValue(lastPath, selectedValue);
        }

        /// <summary>
        /// The following algorithm selects a portion of a compact JSON-LD document using an array of JSON Pointers.
        /// </summary>
        /// <param name="document">A compact JSON-LD document</param>
        /// <param name="pointers">An array of JSON Pointers</param>
        /// <returns>A new JSON-LD document that represents a selection of the original JSON-LD document.</returns>
        /// <remarks>
        /// The document is assumed to use a JSON-LD context that aliases @id and @type to id and type, respectively,
        /// and to use only one @context property at the top level of the document.
        /// </remarks>
        public static JObject? SelectJsonLd(this JObject document, IEnumerable<string> pointers)
        {
            if (!pointers.Any())
            {
                return null;
            }
            var arrays = new List<JArray>();
            var selectionDocument = CreateInitialSelection(document);
            selectionDocument["@context"] = document["@context"];
            foreach (var pointer in pointers)
            {
                var paths = JsonPointerToPaths(pointer);
                SelectPaths(document, paths, selectionDocument, arrays);
            }
            foreach (var array in arrays)
            {
                array.ReplaceAll(array.Where(x => x is not null));
            }
            return selectionDocument;
        }

        /// <summary>
        /// Selects a portion of a skolemized compact JSON-LD document using an array of JSON Pointers, and outputs
        /// the resulting canonical N-Quads with any blank node labels replaced using the given label map.
        /// </summary>
        /// <param name="skolemizedInputDocument">A skolemized compact JSON-LD document.</param>
        /// <param name="jsonPointers">An array of JSON Pointers.</param>
        /// <param name="labelMap">A blank node label map.</param>
        /// <returns>Tuple containing selectionDocument, deskolemizedNQuads, and nquads</returns>
        public static SelectCanonicalNQuadsResult SelectCanonicalNQuads(this JObject skolemizedInputDocument, IEnumerable<string> jsonPointers, IDictionary<string, string> labelMap)
        {
            var selectionDocument = skolemizedInputDocument.SelectJsonLd(jsonPointers)
                ?? throw new ArgumentException("No selection was made.");
            var deskolemizedNQuads = SkolemizationService.ToDeskolemizedNQuads(selectionDocument);
            var relabeledNQuads = SkolemizationService.RelabelBlankNodes(deskolemizedNQuads, labelMap);
            return new SelectCanonicalNQuadsResult
            {
                SelectionDocument = selectionDocument,
                DeskolemizedNQuads = deskolemizedNQuads,
                NQuads = relabeledNQuads
            };
        }

        /// <summary>
        /// Outputs canonical N-Quad strings that match custom selections of a compact JSON-LD document.
        /// </summary>
        /// <param name="document">A compact JSON-LD document.</param>
        /// <param name="labelMapFactoryFunction">A label map factory function.</param>
        /// <param name="groupDefinitions">A map of named group definitions.</param>
        /// <returns>An object containing the created groups, the skolemized compact JSON-LD document, the skolemized
        /// expanded JSON-LD document, the deskolemized N-Quad strings, the blank node label map, and the canonical
        /// N-Quad strings.</returns>
        /// <remarks>
        /// It does this by canonicalizing a compact JSON-LD document (replacing any blank node identifiers using
        /// a label map) and grouping the resulting canonical N-Quad strings according to the selection associated
        /// with each group. Each group will be defined using an assigned name and array of JSON pointers. The JSON
        /// pointers will be used to select portions of the skolemized document, such that the output can be converted
        /// to canonical N-Quads to perform group matching.
        /// </remarks>
        public static CanonicalizationAndGroupingResult CanonicalizeAndGroup(this JObject document, ILabelMapFactoryFunction labelMapFactoryFunction, IDictionary<string, IList<string>> groupDefinitions)
        {
            var (skolemizedExpandedDocument, skolemizedCompactDocument) = SkolemizationService.SkolemizeCompactJsonLd(document, CustomUrnScheme);
            var deskolemizedNQuads = SkolemizationService.ToDeskolemizedNQuads(skolemizedExpandedDocument);
            var deskolemizedTs = new TripleStore();
            deskolemizedTs.LoadFromString(string.Join("\n", deskolemizedNQuads));
            var canonicalizationService = new CanonicalizationService();
            var (canonicalNQuads, labelMap) = canonicalizationService.LabelReplacementCanonicalize(deskolemizedTs, labelMapFactoryFunction);
            var selections = new Dictionary<string, SelectCanonicalNQuadsResult>();
            foreach (var (groupName, jsonPointers) in groupDefinitions)
            {
                var scnr = skolemizedCompactDocument.SelectCanonicalNQuads(jsonPointers, labelMap);
                selections.Add(groupName, scnr);
            }
            var groups = new Dictionary<string, GroupResult>();
            foreach (var (groupName, selection) in selections)
            {
                var selectedNQuads = selection.NQuads;
                var selectedDeskolemizedNQuads = selection.DeskolemizedNQuads;
                var matching = new Dictionary<int, string>();
                var nonMatching = new Dictionary<int, string>();
                for (var i = 0; i < canonicalNQuads.Count; i++)
                {
                    var nQuad = canonicalNQuads[i];
                    if (selectedNQuads.Contains(nQuad))
                    {
                        matching.Add(i, nQuad);
                    }
                    else
                    {
                        nonMatching.Add(i, nQuad);
                    }
                }
                groups.Add(groupName, new GroupResult
                {
                    Matching = matching,
                    NonMatching = nonMatching,
                    DeskolemizedNQuads = selectedDeskolemizedNQuads
                });
            }
            return new CanonicalizationAndGroupingResult
            {
                Groups = groups,
                SkolemizedCompactDocument = skolemizedCompactDocument,
                SkolemizedExpandedDocument = skolemizedExpandedDocument,
                DeskolemizedNQuads = deskolemizedNQuads,
                LabelMap = labelMap,
                CanonicalNQuads = canonicalNQuads
            };
        }

        public static string JsonPointerToJsonPath(string jsonPointer)
        {
            var paths = JsonPointerToPaths(jsonPointer);
            var jsonPath = new StringBuilder();
            foreach (var path in paths)
            {
                if (path.IsT0)
                {
                    jsonPath.Append($".{path.AsT0}");
                }
                else if (path.IsT1)
                {
                    jsonPath.Append($"[{path.AsT1}]");
                }
            }
            return jsonPath.ToString();
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

        private static void SetValue(this JToken jToken, OneOf<string, int> key, JToken value)
        {
            if (key.IsT0)
            {
                var added = ((JObject)jToken).TryAdd(key.AsT0, value);
                if (!added)
                {
                    ((JObject)jToken)[key.AsT0] = value;
                }
            }
            else if (key.IsT1)
            {
                if (jToken.Type == JTokenType.Array)
                {
                    ((JArray)jToken)[key.AsT1] = value;
                }
            }
        }
    }
}
