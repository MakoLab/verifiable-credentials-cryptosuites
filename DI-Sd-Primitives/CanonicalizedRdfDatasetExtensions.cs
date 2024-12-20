using DI_Sd_Primitives.Interfaces;
using OneOf.Types;
using System.Runtime.Intrinsics.X86;
using VDS.RDF;
using static VDS.RDF.RdfCanonicalizer;

namespace DI_Sd_Primitives
{
    internal static class CanonicalizedRdfDatasetExtensions
    {
        internal static IList<string> SerializeWithLabelReplacement(this CanonicalizedRdfDataset crd, ILabelMapFactoryFunction labelMapFactoryFunction)
        {
            var labelMap = labelMapFactoryFunction.CreateLabelMap(crd.IssuedIdentifiersMap);
            // Note: In this current implementation, the replacement label map is
            // replaced with one that maps the C14N labels to the new labels instead of
            // the input labels to the new labels.This is because the C14N labels are
            // already in use in the N-Quads that are updated.
            var c14nToNewLabelMap = new Dictionary<string, string>();
            foreach (var entry in labelMap)
            {
                if (crd.IssuedIdentifiersMap.TryGetValue(entry.Key, out string? c14n))
                {
                    var internalId = c14n;
                    if (c14n.StartsWith("_:"))
                    {
                        internalId = c14n[2..];
                    }
                    c14nToNewLabelMap.Add(internalId, entry.Value);
                }
            }
            var formatter = new NQuadsReplacementFormatter(c14nToNewLabelMap);
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
