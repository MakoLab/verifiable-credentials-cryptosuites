using Cryptosuite.Core;
using JsonLdExtensions;
using VDS.RDF.JsonLd;

namespace SecurityDocumentLoader
{
    public class SecurityDocumentLoader : JsonLdDocumentLoader
    {
        public SecurityDocumentLoader()
        {
            AddStatic(Contexts.Ed25519Signature2020ContextUrl, Contexts.Ed25519Signature2020Context);
            AddStatic(Contexts.X25519KeyAgreement2020V1ContextUrl, Contexts.X25519KeyAgreement2020V1Context);
            AddStatic(Contexts.CredentialsContextV1Url, Contexts.CredentialsContextV1);
            AddStatic(Contexts.DidContextV1Url, Contexts.DidContextV1);
            AddStatic(Contexts.VeresOneContextV1Url, Contexts.VeresOneContextV1);
            AddStatic(Contexts.DataIntegrityV1Url, Contexts.DataIntegrityV1);
            AddStatic(Contexts.DataIntegrityV2Url, Contexts.DataIntegrityV2);
        }

        public override RemoteDocument LoadDocument(Uri url, JsonLdLoaderOptions options)
        {
            if (_documents.TryGetValue(url.AbsoluteUri, out var document))
            {
                return new RemoteDocument()
                {
                    DocumentUrl = url,
                    Document = document,
                };
            }
            throw new ArgumentException($"{nameof(SecurityDocumentLoader)} supports static documents only.");
        }
    }
}
