using System.Security.Authentication;
using VDS.RDF.JsonLd;

namespace JsonLdExtensions.Canonicalization
{
    public class JsonLdNormalizerOptions : JsonLdProcessorOptions
    {
        public bool SkipExpansion { get; set; } = false;
        public bool SafeMode { get; set; } = true;
        public HashAlgorithmType HashAlgorithm { get; set; } = HashAlgorithmType.Sha256;
    }
}
