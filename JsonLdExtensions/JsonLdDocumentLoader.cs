using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF.JsonLd;

namespace JsonLdExtensions
{
    public class JsonLdDocumentLoader : IDocumentLoader
    {
        protected readonly Dictionary<string, JToken> _documents = new();

        public void AddStatic(string url, JToken document)
        {
            _documents.Add(url, document);
        }

        public virtual RemoteDocument LoadDocument(Uri url)
        {
            return LoadDocument(url, new JsonLdLoaderOptions());
        }

        public virtual RemoteDocument LoadDocument(Uri url, JsonLdLoaderOptions options)
        {
            if (_documents.TryGetValue(url.AbsoluteUri, out var document))
            {
                return new RemoteDocument()
                {
                    DocumentUrl = url,
                    Document = document,
                };
            }
            return DefaultDocumentLoader.LoadJson(url, new JsonLdLoaderOptions());
        }
    }
}
