using JsonLD.Core;
using Newtonsoft.Json.Linq;

namespace Cryptosuite.Core.Interfaces
{
    public interface ICryptosuite
    {
        string RequiredAlgorithm { get; }
        string Name { get; }
        Verifier CreateVerifier(VerificationMethod verificationMethod);
    }
}