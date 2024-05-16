using AngleSharp.Dom;
using Microsoft.Json.Pointer;
using Newtonsoft.Json.Linq;
using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI_Sd_Primitives
{
    public static class JsonSelectionExtension
    {
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
