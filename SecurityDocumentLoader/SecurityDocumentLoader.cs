using JsonLdExtensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF.JsonLd;

namespace SecurityDocumentLoader
{
    public class SecurityDocumentLoader : JsonLdDocumentLoader
    {
        public SecurityDocumentLoader()
        {
            AddStatic(Constants.Ed25519Signature2020ContextUrl, Contexts.Ed25519Signature2020Context);
            AddStatic(Constants.X25519KeyAgreement2020V1ContextUrl, Contexts.X25519KeyAgreement2020V1Context);
            AddStatic(Constants.CredentialsContextV1Url, Contexts.CredentialsContextV1);
            AddStatic(Constants.DidContextV1Url, Contexts.DidContextV1);
            AddStatic(Constants.VeresOneContextV1Url, Contexts.VeresOneContextV1);
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
