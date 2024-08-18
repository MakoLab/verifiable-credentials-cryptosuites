using Cryptosuite.Core.ControllerDocuments;

namespace Cryptosuite.Core.Interfaces
{
    public interface ICreateVerifier
    {
        Verifier CreateVerifier(VerificationMethod verificationMethod);
    }
}
