using Cryptosuite.Core;
using JsonLdExtensions;

namespace DataIntegrity
{
    public class VCDIDocumentLoader : JsonLdDocumentLoader
    {
        public VCDIDocumentLoader()
        {
            AddStatic(Contexts.Ed25519Signature2020ContextUrl, Contexts.Ed25519Signature2020Context);
            AddStatic(Contexts.X25519KeyAgreement2020V1ContextUrl, Contexts.X25519KeyAgreement2020V1Context);
            AddStatic(Contexts.CredentialsContextV1Url, Contexts.CredentialsContextV1);
            AddStatic(Contexts.CredentialsContextV2Url, Contexts.CredentialsContextV2);
            AddStatic(Contexts.DidContextV1Url, Contexts.DidContextV1);
            AddStatic(Contexts.VeresOneContextV1Url, Contexts.VeresOneContextV1);
            AddStatic(Contexts.DataIntegrityV1Url, Contexts.DataIntegrityV1);
            AddStatic(Contexts.DataIntegrityV2Url, Contexts.DataIntegrityV2);
        }
    }
}
