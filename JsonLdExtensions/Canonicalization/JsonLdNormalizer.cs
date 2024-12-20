using JsonFlatten;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using VDS.RDF;
using VDS.RDF.JsonLd;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;

namespace JsonLdExtensions.Canonicalization
{
    public static class JsonLdNormalizer
    {
        public static RdfCanonicalizer.CanonicalizedRdfDataset Normalize(JToken input, JsonLdNormalizerOptions? options = null)
        {
            options ??= new JsonLdNormalizerOptions();
            options.ProduceGeneralizedRdf = false;
            var ts = new TripleStore();
            var parser = new JsonLdParser(options);
            parser.Warning += HandleParserWarning;
            var expanded = JsonLdProcessor.Expand(input, options);
            if (options.SkipExpansion)
            {
                ts.LoadFromString(input.ToString(), parser);
            }
            else
            {
                ts.LoadFromString(expanded.ToString(), parser);
            }
            //var writer = new JsonLdWriter(new JsonLdWriterOptions() { UseNativeTypes = true, ProcessingMode = VDS.RDF.JsonLd.Syntax.JsonLdProcessingMode.JsonLd11 });
            //var loaded = writer.SerializeStore(ts);
            //var iExp = JsonLdProcessor.Expand(input, options);
            //var iTypes = (iExp[0]["@type"] as JArray)?.Count;
            //var lTypes = (loaded[0]["@type"] as JArray)?.Count;
            //if ((lTypes is not null) && (lTypes != iTypes))
            //{
            //    throw new DataLossException("Undefined type in input document.");
            //}
            return ts.Canonicalize(options);
        }

        private static void HandleParserWarning(string message)
        {
            Console.WriteLine($"Warning: {message}");
            Debug.WriteLine($"Warning: {message}");
        }
    }
}
