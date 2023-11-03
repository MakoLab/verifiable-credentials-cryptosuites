using JsonLD.Core;
using JsonLdDocumentLoader;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityDocumentLoader
{
    public class SecurityDocumentLoader : JsonLdDocumentLoader.JsonLdDocumentLoader
    {
        public SecurityDocumentLoader()
        {
            AddStatic(Constants.Ed25519Signature2020ContextUrl, Contexts.Ed25519Signature2020Context);
            AddStatic(Constants.X25519KeyAgreement2020V1ContextUrl, Contexts.X25519KeyAgreement2020V1Context);
            AddStatic(Constants.CredentialsContextV1Url, Contexts.CredentialsContextV1);
            AddStatic(Constants.DidContextV1Url, Contexts.DidContextV1);
            AddStatic(Constants.VeresOneContextV1Url, Contexts.VeresOneContextV1);
        }

        public override RemoteDocument LoadDocument(string url)
        {
            if (_documents.TryGetValue(url, out var document))
            {
                return new RemoteDocument(url, document);
            }
            throw new ArgumentException($"{nameof(SecurityDocumentLoader)} supports static documents only.");
        }
    }
}
