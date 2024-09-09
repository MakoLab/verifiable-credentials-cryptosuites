using Cryptosuite.Core.ControllerDocuments;
using System.Diagnostics.CodeAnalysis;

namespace ECDsa_Multikey
{
    public class MultikeyVerificationMethod : VerificationMethod
    {
        public string? PublicKeyMultibase { get; set; }
        public string? SecretKeyMultibase { get; set; }

        [SetsRequiredMembers]
        public MultikeyVerificationMethod()
        {
            Type = "Multikey";
        }
    }
}
