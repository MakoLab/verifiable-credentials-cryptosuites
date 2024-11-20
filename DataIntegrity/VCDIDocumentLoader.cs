using Cryptosuite.Core;
using JsonLdExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VDS.RDF.JsonLd;

namespace DataIntegrity
{
    /// <summary>
    /// A document loader that supports loading documents from HTTP(S) URLs and DID URLs.
    /// </summary>
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

        public override RemoteDocument LoadDocument(Uri url)
        {
            return LoadDocument(url, new JsonLdLoaderOptions());
        }

        public override RemoteDocument LoadDocument(Uri url, JsonLdLoaderOptions options)
        {
            var scheme = url.Scheme;
            if (scheme.StartsWith("http"))
            {
                return base.LoadDocument(url, options);
            }
            if (scheme == "did")
            {
                if (url.LocalPath.StartsWith("key:"))
                {
                    var documentCreator = new DidDocumentCreator();
                    var controllerDocument = documentCreator.CreateControllerDocument(url);
                    return new RemoteDocument()
                    {
                        ContextUrl = null,
                        Document = JObject.FromObject(controllerDocument),
                        DocumentUrl = url,
                    };
                }
                throw new ArgumentException("Only 'key' method is supported.");
            }
            throw new ArgumentException("Only 'http' and 'did' schemes are supported.");
        }
    }
}
