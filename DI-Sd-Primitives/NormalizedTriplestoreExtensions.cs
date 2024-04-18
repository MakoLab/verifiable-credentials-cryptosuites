using DI_Sd_Primitives.Interfaces;
using JsonLdExtensions.Canonicalization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI_Sd_Primitives
{
    internal static class NormalizedTriplestoreExtensions
    {
        internal static List<string> SerializeWithLabelReplacement(this NormalizedTriplestore nts, ILabelMapFactoryFunction labelMapFactoryFunction)
        {
            var dict = nts.IssuedIdentifiers.ToDictionary();
            var labelMap = labelMapFactoryFunction.CreateLabelMap(dict);
            var formatter = new NQuadsReplacementFormatter(labelMap);
            var lines = new List<string>();
            foreach (var quad in nts.Store.GetQuads())
            {
                lines.Add(formatter.Format(quad.Triple, quad.Graph.Name));
            }
            lines.Sort();
            return lines;
        }

        internal static Dictionary<string, string> ToDictionary(this OrderedDictionary od)
        {
            var dict = new Dictionary<string, string>();
            foreach (var key in od.Keys)
            {
                dict.Add((string)key, (string)od[key]!);
            }
            return dict;
        }
    }
}
