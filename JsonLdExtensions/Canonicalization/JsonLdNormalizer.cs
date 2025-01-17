using Newtonsoft.Json.Linq;
using JsonLdExtensions;
using VDS.RDF;
using VDS.RDF.JsonLd;
using VDS.RDF.Parsing;

namespace JsonLdExtensions.Canonicalization
{
    public static class JsonLdNormalizer
    {
        public static RdfCanonicalizer.CanonicalizedRdfDataset Normalize(JToken input, JsonLdNormalizerOptions? options = null)
        {
            options ??= new JsonLdNormalizerOptions();
            options.ProduceGeneralizedRdf = false;
            options.SafeMode = true;
            var ts = new TripleStore();
            var parser = new JsonLdParser(options);
            var processorWarnings = new List<JsonLdProcessorWarning>();
            var expanded = JsonLdProcessor.Expand(input, options, processorWarnings);
            if (processorWarnings.Any())
            {
                ts.Dispose();
                throw new DataLossException(String.Join("\n", processorWarnings.Select(w => w.Message)));
            }
            if (options.SkipExpansion)
            {
                ts.SafeLoadFromString(input.ToString(), parser);
            }
            else
            {
                ts.SafeLoadFromString(expanded.ToString(), parser);
            }
            return ts.Canonicalize(options);
        }
    }
}
