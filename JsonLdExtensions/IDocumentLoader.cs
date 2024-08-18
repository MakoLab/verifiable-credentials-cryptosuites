using VDS.RDF.JsonLd;

namespace JsonLdExtensions
{
    public interface IDocumentLoader
    {
        public RemoteDocument LoadDocument(Uri url);
        public RemoteDocument LoadDocument(Uri url, JsonLdLoaderOptions options);
    }
}
