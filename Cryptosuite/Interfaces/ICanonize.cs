using JsonLdExtensions.Canonicalization;
using Newtonsoft.Json.Linq;

namespace Cryptosuite.Core.Interfaces
{
    public interface ICanonize
    {
        string Canonize(JToken input, JsonLdNormalizerOptions options);
    }
}
