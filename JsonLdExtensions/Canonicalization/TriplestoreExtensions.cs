using System.Security.Authentication;
using VDS.RDF;
using VDS.RDF.Writing.Formatting;

namespace JsonLdExtensions.Canonicalization
{
    public static class TriplestoreExtensions
    {
        private static string HashAlgorithm { get; set; } = "SHA256";

        public static RdfCanonicalizer.CanonicalizedRdfDataset Canonicalize(this ITripleStore ts, JsonLdNormalizerOptions? options = null)
        {
            if (ts is null)
            {
                throw new ArgumentNullException(nameof(ts));
            }
            options ??= new JsonLdNormalizerOptions();
            if (options.HashAlgorithm != HashAlgorithmType.Sha256)
            {
                HashAlgorithm = options.HashAlgorithm switch
                {
                    HashAlgorithmType.Sha384 => "SHA384",
                    HashAlgorithmType.Sha512 => "SHA512",
                    _ => throw new ArgumentException($"Invalid hash algorithm {nameof(options.HashAlgorithm)}")
                };
            }
            var canonicalizer = new RdfCanonicalizer(HashAlgorithm);
            return canonicalizer.Canonicalize(ts);
        }

        public static bool Add(this ITripleStore ts, Quad quad)
        {
            if (quad is null)
            {
                return false;
            }
            var graph = new Graph(quad.Graph);
            graph.Assert(quad.Subject, quad.Predicate, quad.Object);
            return ts.Add(graph, mergeIfExists: true);
        }

        public static IEnumerable<INode?> GetComponents(this Quad quad)
        {
            return new INode?[] { quad.Subject, quad.Predicate, quad.Object, quad.Graph };
        }

        public static string ToNQuad(this Quad quad, NQuadsFormatter formatter)
        {
            return formatter.Format(quad.AsTriple(), quad.Graph);
        }
    }
}
