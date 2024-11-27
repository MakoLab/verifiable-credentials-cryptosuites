using VDS.RDF.JsonLd;

namespace JsonLdExtensions
{
    public enum UriType
    {
        Http,
        Did,
        Neither,
    }

    public interface IDocumentLoader
    {
        public UriType GetUriType(Uri url) => url.Scheme switch
        {
            "http" or "https" => UriType.Http,
            "did" => UriType.Did,
            _ => UriType.Neither
        };

        public RemoteDocument LoadDocument(Uri url);
        public RemoteDocument LoadDocument(Uri url, JsonLdLoaderOptions options);
    }
}
