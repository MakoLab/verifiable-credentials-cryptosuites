using DataIntegrity;
using JsonLdExtensions;
using VDS.RDF.JsonLd;

namespace SecurityTestDocumentLoader
{
    public class SecurityDocumentLoader : VCDIDocumentLoader
    {
        public SecurityDocumentLoader(IDidDocumentCreator didDocumentCreator) : base(didDocumentCreator)
        {
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

            if (((IDocumentLoader)this).GetUriType(url) == UriType.Did)
            {
                return base.LoadDocument(url, options);
            }
            throw new ArgumentException($"{nameof(SecurityDocumentLoader)} supports static http documents only.");
        }
    }
}
