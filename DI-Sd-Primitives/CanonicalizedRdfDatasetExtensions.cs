using DI_Sd_Primitives.Interfaces;
using JsonLdExtensions.Canonicalization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VDS.RDF.RdfCanonicalizer;

namespace DI_Sd_Primitives
{
    internal static class CanonicalizedRdfDatasetExtensions
    {
        internal static IList<string> SerializeWithLabelReplacement(this CanonicalizedRdfDataset crd, ILabelMapFactoryFunction labelMapFactoryFunction)
        {
            var labelMap = labelMapFactoryFunction.CreateLabelMap(crd.IssuedIdentifiersMap);
            var formatter = new NQuadsReplacementFormatter(labelMap);
            var lines = new List<string>();
            foreach (var quad in crd.OutputDataset.Quads)
            {
                lines.Add(formatter.Format(quad.AsTriple(), quad.Graph));
            }
            lines.Sort();
            return lines;
        }
    }
}
