using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF.JsonLd;

namespace JsonLdExtensions.Canonicalization
{
    public class JsonLdNormalizerOptions : JsonLdProcessorOptions
    {
        public bool SkipExpansion { get; set; } = false;
        public HashAlgorithmType HashAlgorithm { get; set; } = HashAlgorithmType.Sha256;
    }
}
