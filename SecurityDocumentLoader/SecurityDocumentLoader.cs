using JsonLdExtensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF.JsonLd;
using Cryptosuite.Core;

namespace SecurityDocumentLoader
{
    public class SecurityDocumentLoader : JsonLdDocumentLoader
    {
        public SecurityDocumentLoader()
        {
            AddStatic(SecurityConstants.Ed25519Signature2020ContextUrl, Contexts.Ed25519Signature2020Context);
            AddStatic(SecurityConstants.X25519KeyAgreement2020V1ContextUrl, Contexts.X25519KeyAgreement2020V1Context);
            AddStatic(SecurityConstants.CredentialsContextV1Url, Contexts.CredentialsContextV1);
            AddStatic(SecurityConstants.DidContextV1Url, Contexts.DidContextV1);
            AddStatic(SecurityConstants.VeresOneContextV1Url, Contexts.VeresOneContextV1);
            AddStatic(SecurityConstants.DataIntegrityV1Url, Contexts.DataIntegrityV1);
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
