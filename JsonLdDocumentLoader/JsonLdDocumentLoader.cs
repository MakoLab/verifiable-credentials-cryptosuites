using JsonLD.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonLdDocumentLoader
{
    public class JsonLdDocumentLoader : DocumentLoader
    {
        protected readonly Dictionary<string, JToken> _documents = new();

        public void AddStatic(string url, JToken document)
        {
            _documents.Add(url, document);
        }

        public override RemoteDocument LoadDocument(string url)
        {
            if (_documents.TryGetValue(url, out var document))
            {
                return new RemoteDocument(url, document);
            }
            return base.LoadDocument(url);
        }

        public override Task<RemoteDocument> LoadDocumentAsync(string url)
        {
            if (_documents.TryGetValue(url, out var document))
            {
                return Task.FromResult(new RemoteDocument(url, document));
            }
            return base.LoadDocumentAsync(url);
        }
    }
}
