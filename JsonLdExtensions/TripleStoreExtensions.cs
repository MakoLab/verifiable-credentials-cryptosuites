using JsonLdExtensions.Canonicalization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.JsonLd;
using VDS.RDF.Parsing;

namespace JsonLdExtensions
{
    public static class TripleStoreExtensions
    {
        private static List<string> Warnings = new();

        public static void SafeLoadFromString(this TripleStore ts, string data, JsonLdParser parser)
        {
            parser.Warning += HandleParserWarning;
            Warnings.Clear();
            ts.LoadFromString(data, parser);
            if (Warnings.Any())
            {
                ts.Dispose();
                throw new DataLossException(String.Join("\n", Warnings));
            }
        }

        private static void HandleParserWarning(string message)
        {
            Debug.WriteLine($"Warning: {message}");
            Warnings.Add(message);
        }
    }
}
