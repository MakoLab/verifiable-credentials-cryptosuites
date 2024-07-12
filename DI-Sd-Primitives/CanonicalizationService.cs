using DI_Sd_Primitives.Interfaces;
using JsonLdExtensions.Canonicalization;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Parsing;

namespace DI_Sd_Primitives
{
    public class CanonicalizationService
    {
        public (IList<string> canonicalNQuads, IDictionary<string, string> labelMap) LabelReplacementCanonicalize(ITripleStore ts,
            ILabelMapFactoryFunction labelFunction, JsonLdNormalizerOptions? options = null)
        {
            var crd = ts.Canonicalize(options);
            var canonicalNQuads = crd.SerializeWithLabelReplacement(labelFunction);
            return (canonicalNQuads, crd.IssuedIdentifiersMap);
        }

        public (IList<string> canonicalNQuads, IDictionary<string, string> labelMap) LabelReplacementCanonicalize(JToken json,
            ILabelMapFactoryFunction labelFunction, JsonLdNormalizerOptions? options = null)
        {
            var nts = JsonLdNormalizer.Normalize(json, options);
            var canonicalNQuads = nts.SerializeWithLabelReplacement(labelFunction);
            return (canonicalNQuads, nts.IssuedIdentifiersMap);
        }
    }
}
