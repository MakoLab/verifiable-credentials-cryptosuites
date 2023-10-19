using JsonLD.Core;
using Newtonsoft.Json.Linq;

namespace Cryptosuite
{
    public interface ICryptosuite
    {
        string RequiredAlgorithm { get; }
        string Name { get; }
        Verifier CreateVerifier(string publicKey);
        object Canonize(JToken input, JsonLdOptions options);
    }
}