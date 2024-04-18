using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.JsonLd;
using VDS.RDF.Parsing;

namespace JsonLdExtensions.Canonicalization
{
    public static class JsonLdNormalizer
    {
        public static NormalizedTriplestore Normalize(JToken input, JsonLdNormalizerOptions? options = null)
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
            return ts.Normalize(options);
        }
    }
}
