using Newtonsoft.Json.Linq;
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
            var ts = new TripleStore();
            var parser = new JsonLdParser(options);
            if (options.SkipExpansion)
            {
                ts.LoadFromString(input.ToString(), parser);
            }
            else
            {
                var expanded = JsonLdProcessor.Expand(input, options);
                ts.LoadFromString(expanded.ToString(), parser);
            }
            return ts.Canonicalize(options);
        }
    }
}
