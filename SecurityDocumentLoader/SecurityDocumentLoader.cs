using DataIntegrity;
using VDS.RDF.JsonLd;

namespace SecurityDocumentLoader
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
            throw new ArgumentException($"{nameof(SecurityDocumentLoader)} supports static documents only.");
        }
    }
}
